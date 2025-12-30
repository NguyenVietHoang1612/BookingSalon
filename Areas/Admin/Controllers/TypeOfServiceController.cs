using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TypeOfServiceController : Controller
    {
        private readonly ITypeOfServiceService _typeOfServiceService;

        public TypeOfServiceController(ITypeOfServiceService typeOfServiceService)
        {
            _typeOfServiceService = typeOfServiceService;
        }

        public async Task<IActionResult> Index(string term)
        {
            var listTypeOfService = await _typeOfServiceService.GetAllServiceAsync();

            if (!string.IsNullOrEmpty(term))
            {
                listTypeOfService = listTypeOfService.Where(f => f.Type_Service_Name.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
                ViewBag.SearchTerm = term;
            }

            return View(listTypeOfService);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TypeOfService typeOfService)
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

            var result = await _typeOfServiceService.CreateAsync(typeOfService);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.Errors;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(typeOfService);

            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var typeOfService = await _typeOfServiceService.GetByIdAsync(id);

            if (typeOfService == null) return NotFound();

            TypeOfService tOS = new TypeOfService
            {
                Type_Service_Name = typeOfService.Data.Type_Service_Name
            };

            return View(tOS);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, TypeOfService typeOfService)
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

            var result = await _typeOfServiceService.UpdateAsync(id, typeOfService);

            if (!result.Succeeded)
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View(typeOfService);

            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _typeOfServiceService.DeleteAsync(id);
            return Ok(new { message = "Đã chuyển trạng thái user sang ngừng hoạt động" });
        }
    }
}
