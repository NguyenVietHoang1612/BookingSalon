using BookingSalon.Models.Entities;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] 
    public class StaffPortfolioController : Controller
    {
        private readonly IStaffPortfolioService _portfolioService;

        public StaffPortfolioController(IStaffPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex(int? pageNumber, string term)
        {
            var pagedData = await _portfolioService.GetPagedListAsync(pageNumber ?? 1, 15, term);
            ViewBag.SearchTerm = term;
            return View(pagedData);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> UpdateIsFeatured(int id, bool isFeatured)
        {
            var result = await _portfolioService.UpdateIsFeaturedAsync(id, isFeatured);

            if (result.Succeeded)
            {
                TempData["Success"] = isFeatured ? "Đã duyệt ảnh thành công!" : "Đã hủy duyệt ảnh!";
            }
            else
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
            }

            return RedirectToAction(nameof(AdminIndex));
        }


        [HttpGet]
        [Authorize(Roles = "Stylist, Skinner")]
        public async Task<IActionResult> StaffIndex(int? pageNumber, string term)
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int pageSize = 10; 
            var pagedData = await _portfolioService.GetStaffPagedListAsync(staffId, pageNumber ?? 1, pageSize, term);

            ViewBag.SearchTerm = term;
            return View(pagedData);
        }

        [HttpGet]
        public async Task<IActionResult> StaffCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Stylist, Skinner")]
        public async Task<IActionResult> StaffCreate([Bind("ImageId,Category,Title,Staff_Portfolio_Image_Upload")] StaffPortfolioModel model)
        {
            model.StaffProfileId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(Enum.GetValues(typeof(PortfolioCategory)));
                return View(model);
            }

            var result = await _portfolioService.CreateAsync(model);
            if (result.Succeeded)
            {
                TempData["Success"] = "Tải lên thành công! Vui lòng chờ Admin duyệt để hiển thị công khai.";
                return RedirectToAction(nameof(StaffIndex));
            }
            return View(model);
        }

        public async Task<IActionResult> StaffUpdate(int id)
        {
            var result = await _portfolioService.GetByIdAsync(id);
            if (!result.Succeeded || result.Data == null) return NotFound();

            var portfolio = result.Data;

            return View(portfolio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Stylist, Skinner")]
        public async Task<IActionResult> StaffUpdate(int id, [Bind("ImageId,Category,Title,Staff_Portfolio_Image_Upload")] StaffPortfolioModel model)
        {
            var currentStaffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var checkOwn = await _portfolioService.GetByIdAsync(id);

            if (checkOwn.Data == null || checkOwn.Data.StaffProfileId != currentStaffId)
            {
                return Forbid(); 
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(Enum.GetValues(typeof(PortfolioCategory)));
                return View(model);
            }

            var result = await _portfolioService.UpdateAsync(id, model);
            if (result.Succeeded)
            {
                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(StaffIndex));
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Stylist, Skinner")]
        public async Task<IActionResult> StaffDelete(int id)
        {
            var currentStaffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await _portfolioService.GetByIdAsync(id);

            if (item.Data?.StaffProfileId != currentStaffId) return BadRequest();

            var result = await _portfolioService.DeleteAsync(id);

            if (result.Succeeded)
            {
                return Ok(new { message = "Xóa ảnh thành công" });
            }

            return BadRequest(new { success = false, message = "Không thể xóa dữ liệu" });
            
        }
    }
}