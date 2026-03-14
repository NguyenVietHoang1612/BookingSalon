using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSalon.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReview(ReviewRequestVM request) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _reviewService.SubmitReviewAsync(
                request.BookingId,
                request.Rating,
                request.Comment,
                userId
            );

            if (result.Succeeded) return Json(new { success = true });
            return Json(new { success = false, message = result.Errors.FirstOrDefault() });
        }

        public async Task<IActionResult> Index(int? pageNumber, string term)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.SearchTerm = term;
            int pageSize = 10;
            var result = await _reviewService.GetCustomerPagedReviewsAsync(userId, pageNumber ?? 1, pageSize, term);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);
            if (result.Succeeded) return Ok(new { message = "Xóa thành công" });
            return BadRequest(new { message = result.Errors.FirstOrDefault() });
        }
    }
}
