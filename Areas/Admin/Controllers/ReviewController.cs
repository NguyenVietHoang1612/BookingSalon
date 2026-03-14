using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Stylist, Skinner")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int? pageNumber, string term)
        {
            int pageSize = 10;
            var result = await _reviewService.GetPagedReviewsAsync(pageNumber ?? 1, pageSize, term);
            ViewBag.SearchTerm = term;
            return View(result);
        }

        [Authorize(Roles = "Stylist, Skinner")]
        public async Task<IActionResult> StaffIndex(int? pageNumber, string term)
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int pageSize = 10;
            var result = await _reviewService.GetStaffPagedReviewsAsync(userLogin, pageNumber ?? 1, pageSize, term);
            ViewBag.SearchTerm = term;
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reply(int id, string replyComment)
        {
            if (string.IsNullOrEmpty(replyComment))
            {
                TempData["Error"] = "Nội dung phản hồi không được để trống";
                return RedirectToAction(nameof(Index));
            }

            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewService.ReplyReviewAsync(id, replyComment, adminId);

            if (result.Succeeded)
                TempData["Success"] = "Đã gửi phản hồi cho khách hàng!";
            else
                TempData["Error"] = result.Errors.FirstOrDefault();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);
            if (result.Succeeded) return Ok(new { message = "Xóa thành công" });
            return BadRequest(new { message = result.Errors.FirstOrDefault() });
        }
    }
}
