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

        public async Task<IActionResult> Index(string term)
        {
            var fixedTimeSlots = await _fixedTimeSlotService.GetAllListAsync();

            if (!string.IsNullOrEmpty(term))
            {
                fixedTimeSlots = fixedTimeSlots.Where(f => f.TimeLabel.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
                ViewBag.SearchTerm = term;
            }

            return View(fixedTimeSlots);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FixedTimeSlot fixedTime)
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

            var result = await _fixedTimeSlotService.CreateAsync(fixedTime);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.Errors;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(fixedTime);

            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            var fixTimeSlot = await _fixedTimeSlotService.GetByIdAsync(id);

            if (fixTimeSlot == null) return NotFound();

            FixedTimeSlot fixTimeDetail = new FixedTimeSlot
            {
                TimeLabel = fixTimeSlot.Data.TimeLabel,
                Sort_Order = fixTimeSlot.Data.Sort_Order
            };

            return View(fixTimeDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, FixedTimeSlot fixedTime)
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

            var result = await _fixedTimeSlotService.UpdateAsync(id, fixedTime);

            if (!result.Succeeded)
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View(fixedTime);

            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _fixedTimeSlotService.DeleteAsync(id);
            return Ok(new { message = "Đã chuyển trạng thái user sang ngừng hoạt động" });
        }
    }
}
