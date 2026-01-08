using BookingSalon.Models.Entities;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index(int? pageNumber, string term)
        {
            int pageSize = 10;
            var pagedData = await _bookingService.GetPagedListAsync(pageNumber ?? 1, pageSize, term);

            if (!string.IsNullOrEmpty(term))
            {
                ViewBag.SearchTerm = term;
            }

            return View(pagedData);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _bookingService.DeleteAsync(id);
            return Ok(new { message = "Đã chuyển trạng thái user sang ngừng hoạt động" });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string status)
        {
            if (!ModelState.IsValid)
            {
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                string errorMessage = string.Join("; ", errors);

                return BadRequest(errorMessage);
            }

            var result = await _bookingService.UpdateAsync(id, status);

            if (!result.Succeeded)
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return NotFound();

            }

            return Ok(new { success = true });
        }

        public async Task<IActionResult> BookingDetails(int id)
        {
            var result = await _bookingService.GetAllBookingDetailsAsync(id);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return NotFound();
            }
            return View(result.Data);
        }
    }
}
