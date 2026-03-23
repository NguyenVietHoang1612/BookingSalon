using BookingSalon.Models.Entities;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TypeOfServiceController : Controller
    {
        private readonly ITypeOfServiceService _typeOfServiceService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TypeOfServiceController(ITypeOfServiceService typeOfServiceService, RoleManager<IdentityRole> roleManager)
        {
            _typeOfServiceService = typeOfServiceService;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(int? pageNumber, string term)
        {
            int pageSize = 10;
            var listService = await _typeOfServiceService.GetPagedListAsync(pageNumber ?? 1, pageSize, term);


            if (!string.IsNullOrEmpty(term))
            {
                ViewBag.SearchTerm = term;
            }

            return View(listService);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles
                .Where(r => r.Name == "Skinner" || r.Name == "Stylist")
                .ToListAsync();

            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TypeOfServiceModel typeOfService)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = $"Lỗi Bind dữ liệu loại dịch vụ!";

                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                var roles = await _roleManager.Roles
                    .Where(r => r.Name == "Skinner" || r.Name == "Stylist")
                    .ToListAsync();

                ViewBag.Roles = new SelectList(roles, "Id", "Name");

                string errorMessage = string.Join("; ", errors);

                return BadRequest(errorMessage);
            }

            var result = await _typeOfServiceService.CreateAsync(typeOfService);

            if (!result.Succeeded)
            {
                TempData["Error"] = "Thêm loại dịch vụ thất bại: " + result.Errors;

                var roles = await _roleManager.Roles
                    .Where(r => r.Name == "Skinner" || r.Name == "Stylist")
                    .ToListAsync();

                ViewBag.Roles = new SelectList(roles, "Id", "Name");

                TempData["ErrorMessage"] = result.Errors;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(typeOfService);

            }

            TempData["Success"] = $"Thêm loại dịch vụ {typeOfService.Type_Service_Name} thành công!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var typeOfService = await _typeOfServiceService.GetByIdAsync(id);

            if (typeOfService == null) return NotFound();

            TypeOfServiceModel tOS = new TypeOfServiceModel
            {
                Type_Service_Name = typeOfService.Data.Type_Service_Name,
                AppliedStaffRoleId = typeOfService.Data.AppliedStaffRoleId,

            };

            var roles = await _roleManager.Roles
                .Where(r => r.Name == "Skinner" || r.Name == "Stylist")
                .ToListAsync();

            ViewBag.Roles = new SelectList(roles, "Id", "Name", tOS.AppliedStaffRoleId);

            return View(tOS);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, TypeOfServiceModel typeOfService)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = $"Lỗi Bind dữ liệu loại dịch vụ!";

                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                var roles = await _roleManager.Roles
                    .Where(r => r.Name == "Skinner" || r.Name == "Stylist")
                    .ToListAsync();

                ViewBag.Roles = new SelectList(roles, "Id", "Name");

                string errorMessage = string.Join("; ", errors);

                return BadRequest(errorMessage);
            }

            var result = await _typeOfServiceService.UpdateAsync(id, typeOfService);

            if (!result.Succeeded)
            {
                TempData["Error"] = "Cập nhật loại dịch vụ thất bại: " + result.Errors;

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                var roles = await _roleManager.Roles
                .Where(r => r.Name == "Skinner" || r.Name == "Stylist")
                .ToListAsync();

                ViewBag.Roles = new SelectList(roles, "Id", "Name");

                return View(typeOfService);
            }


            TempData["Success"] = $"Cập nhật loại dịch vụ {typeOfService.Type_Service_Name} thành công!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _typeOfServiceService.DeleteAsync(id);
            if (result.Succeeded)
            {
                return Ok(new { message = "Xóa loại dịch vụ thành công" });
            }

            return BadRequest(new { success = false, message = "Không thể xóa dữ liệu" });
        }
    }
}
