using BookingSalon.Areas.Admin.Models;
using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin ,Reception")]
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;
        private readonly UserManager<UsersModel> _userManager;
        private readonly IFileService _fileService;

        public NewsController(INewsService newsService, UserManager<UsersModel> userManager , IFileService fileService)
        {
            _newsService = newsService;
            _userManager = userManager;
            _fileService = fileService;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int? pageNumber, string term)
        {
            int pageSize = 10;
            var listNews = await _newsService.GetPagedListAsync(pageNumber ?? 1, pageSize, term);

            if (!string.IsNullOrEmpty(term))
            {
                ViewBag.SearchTerm = term;
            }

            return View(listNews);
        }
        [Authorize(Roles = "Reception")]
        public async Task<IActionResult> ReceptionIndex(int? pageNumber, string term)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int pageSize = 10;
            var listNews = await _newsService.GetReceptionPagedListAsync(userId, pageNumber ?? 1, pageSize, term);

            if (!string.IsNullOrEmpty(term))
            {
                ViewBag.SearchTerm = term;
            }

            return View(listNews);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new NewsVM();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsVM newsVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = $"Lỗi Bind dữ liệu tin tức!";
                return View(newsVM);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _newsService.CreateAsync(newsVM, userId);

            if (!result.Succeeded)
            {
                TempData["Error"] = $"Thêm tin tức thất bại: " + result.Errors;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(newsVM);
            }
            TempData["Success"] = "Thêm tin tức thành công!";

            if (User.IsInRole("Reception"))
            {
                return RedirectToAction(nameof(ReceptionIndex));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var result = await _newsService.GetByIdAsync(id);

            if (!result.Succeeded) return NotFound();

            var vm = new NewsVM
            {
                News = result.Data
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _newsService.GetByIdAsync(id);

            if (!result.Succeeded || !result.Data.IsActive)
                return NotFound();

            return View(result.Data);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken] 
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload == null || upload.Length == 0)
                return Json(new { error = new { message = "File không hợp lệ." } });

            var fileName = await _fileService.UploadFileAsync(upload, "news");

            if (string.IsNullOrEmpty(fileName))
            {
                return Json(new { error = new { message = "Lỗi khi lưu file vào server." } });
            }

            var url = $"/media/news/{fileName}";
            return Json(new { url = url });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, NewsVM newsVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = $"Lỗi Bind dữ liệu tin tức!";
                return View(newsVM);
            }

            var result = await _newsService.UpdateAsync(id, newsVM);

            if (!result.Succeeded)
            {
                TempData["Error"] = $"Cập nhật tin tức thất bại " + result.Errors;

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(newsVM);
            }
            TempData["Success"] = "Cập nhật tin tức thành công!";


            if (User.IsInRole("Reception"))
            {
                return RedirectToAction(nameof(ReceptionIndex));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _newsService.DeleteAsync(id);

            if (result.Succeeded)
            {
                return Ok(new { message = "Xóa bài viết thành công" });
            }
            return BadRequest(new { message = "Xóa thất bại" });
        }

        
    }
}
