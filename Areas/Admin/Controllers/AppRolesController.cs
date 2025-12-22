using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AppRolesController : Controller
    {
        private readonly IAppRolesService _appRolesService;

        public AppRolesController(IAppRolesService appRolesService)
        {
            _appRolesService = appRolesService;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            var roles = await _appRolesService.GetAllAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                roles = roles.Where(r => r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                ViewBag.SearchTerm = searchTerm; 
            }

            return View(roles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole role)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Name", "lỗi không đủ dữ liệu!");
                return View(role);
            }

            var result = await _appRolesService.CreateUserAsync(role);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(role);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Update(string id)
        {
            var role = _appRolesService.GetByIdAsync(id).Result;
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Update(IdentityRole role)
        {
            if (!ModelState.IsValid) return View(role);

            var result = await _appRolesService.UpdateUserAsync(role);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(role);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(IdentityRole role)
        {
            var result = await _appRolesService.DeleteAsync(role);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction("Index");
        }


    }
}
