using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RankController : Controller
    {
        private readonly IRankService _rankService;

        public RankController(IRankService rankService)
        {
            _rankService = rankService;
        }

        public async Task<IActionResult> Index(int? pageNumber, string term)
        {
            int pageSize = 10;
            var pagedData = await _rankService.GetPagedListAsync(pageNumber ?? 1, pageSize, term);

            if (!string.IsNullOrEmpty(term))
            {
                ViewBag.SearchTerm = term;
            }

            return View(pagedData);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RankModel rank)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = $"Lỗi Bind dữ liệu xếp hạng!";
                return View(rank);
            }

            var result = await _rankService.CreateAsync(rank);

            if (result.Succeeded)
            {
                TempData["Success"] = $"Thêm hạng thành viên {rank.RankName} thành công!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = $"Thêm hạng thành viên thất bại: " + result.Errors;
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
                    
            }
            return View(rank);
             
        }

        public async Task<IActionResult> Update(int id)
        {
            var result = await _rankService.GetByIdAsync(id);
            if (!result.Succeeded || result.Data == null)
            {
                return NotFound();
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, RankModel rank)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = $"Lỗi Bind dữ liệu xếp hạng!";
                return View(rank);
            }

            var result = await _rankService.UpdateAsync(id, rank);

            if (!result.Succeeded)
            {
                TempData["Error"] = $"Cập nhật hạng thành viên thất bại: " + result.Errors;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(rank);
            }

            TempData["Success"] = "Cập nhật hạng thành viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _rankService.DeleteAsync(id);

            if (!result.Succeeded)
            {
                return Json(new { success = false, message = result.Errors.FirstOrDefault() });
            }

            return Json(new { success = true, message = "Đã xóa hạng thành viên thành công" });
        }
    }
}
