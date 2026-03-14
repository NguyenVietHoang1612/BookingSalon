using BookingSalon.Models.Entities;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FixedTimeSlotController : Controller
    {
        private readonly IFixedTimeSlotService _fixedTimeSlotService;

        public FixedTimeSlotController(IFixedTimeSlotService fixedTimeSlotService)
        {
            _fixedTimeSlotService = fixedTimeSlotService;
        }

        public async Task<IActionResult> Index(int? pageNumber, string term)
        {
            int pageSize = 10;
            var pagedData = await _fixedTimeSlotService.GetPagedListAsync(pageNumber ?? 1, pageSize, term);

            return View(pagedData);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FixedTimeSlotModel fixedTime)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = $"Lỗi Bind dữ liệu khung giờ!";
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

            var result = await _fixedTimeSlotService.CreateAsync(fixedTime);

            if (!result.Succeeded)
            {
                TempData["Error"] = $"Thêm khung giờ thất bại: " + result.Errors;

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(fixedTime);

            }
            TempData["Success"] = "Thêm khung giờ thành công!";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            var fixTimeSlot = await _fixedTimeSlotService.GetByIdAsync(id);

            if (fixTimeSlot == null) return NotFound();

            FixedTimeSlotModel fixTimeDetail = new FixedTimeSlotModel
            {
                TimeLabel = fixTimeSlot.Data.TimeLabel,
            };

            return View(fixTimeDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, FixedTimeSlotModel fixedTime)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = $"Lỗi Bind dữ liệu khung giờ!";

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

            var result = await _fixedTimeSlotService.UpdateAsync(id, fixedTime);

            if (!result.Succeeded)
            {
                TempData["Error"] = $"Cập nhật khung giờ thất bại!" + result.Errors;

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View(fixedTime);

            }
            TempData["Success"] = "Cập nhật khung giờ thành công!";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _fixedTimeSlotService.DeleteAsync(id);

            if (result.Succeeded)
            {
                return Ok(new { success = true, message = "Xóa thành công" });
            }

            return BadRequest(new { success = false, message = "Không thể xóa dữ liệu" });
        }
    }
}
