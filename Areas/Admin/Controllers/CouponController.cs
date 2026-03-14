using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Reception")]
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> Index(int? pageNumber, string term)
        {
            int pageSize = 10;

            var pagedData = await _couponService
                .GetPagedListAsync(pageNumber ?? 1, pageSize, term);

            if (!string.IsNullOrEmpty(term))
                ViewBag.SearchTerm = term;

            return View(pagedData);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CouponModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = "Lỗi bind dữ liệu coupon!";
                return View(model);
            }

            var result = await _couponService.CreateAsync(model);

            if (result.Succeeded)
            {
                TempData["Success"] = $"Tạo coupon {model.Code} thành công!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Tạo coupon thất bại!";
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            var result = await _couponService.GetByIdAsync(id);

            if (!result.Succeeded || result.Data == null)
                return NotFound();

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CouponModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = "Lỗi bind dữ liệu coupon!";
                return View(model);
            }

            var result = await _couponService.UpdateAsync(id, model);

            if (!result.Succeeded)
            {
                TempData["Error"] = "Cập nhật coupon thất bại!";
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(model);
            }

            TempData["Success"] = "Cập nhật coupon thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ValidateCouponAjax(string code, decimal orderAmount, string userId)
        {
            var targetUserId = !string.IsNullOrEmpty(userId) ? userId : User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _couponService
                .ValidateForBookingAsync(code, orderAmount, targetUserId);

            if (!result.Succeeded)
                return Json(new { succeeded = false, errors = result.Errors });

            return Json(new
            {
                succeeded = true,
                discount = result.Data.Discount
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _couponService.DeleteAsync(id);

            if (!result.Succeeded)
            {
                return Json(new
                {
                    success = false,
                    message = result.Errors.FirstOrDefault()
                });
            }

            return Json(new
            {
                success = true,
                message = "Đã xóa coupon thành công"
            });
        }
    }
}