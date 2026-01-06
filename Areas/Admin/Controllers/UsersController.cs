using BookingSalon.Areas.Admin.Models;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(IUsersService usersService, RoleManager<IdentityRole> roleManager)
        {
            _usersService = usersService;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _usersService.GetAllAsync();
            var roles = _roleManager.Roles.ToList();

            var model = users.Select(u => new UserViewModel
            {
                User = u,
                RoleName = roles.FirstOrDefault(r => r.Id == u.RoleId)?.Name ?? "N/A"
            });
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            var roles = await _roleManager.Roles.Where(r => r.Id != adminRole.Id).ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new UserCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel userVM)
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

            var result = await _usersService.CreateUserAsync(userVM);

            if (!result.Succeeded)
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(userVM);
                
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(string id)
        {
            var user = await _usersService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var adminRole = await _roleManager.FindByNameAsync("Admin");
            var roles = await _roleManager.Roles.Where(r => r.Id != adminRole.Id).ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            UserUpdateViewModel accountUserViewModel = new UserUpdateViewModel
            {
                User = user,
            };


            return View(accountUserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserUpdateViewModel userVM, string id)
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

            var result = await _usersService.UpdateUserAsync(userVM, id);

            if (!result.Succeeded)
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(userVM);

            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SoftDelete(string id)
        {
            await _usersService.SoftDeleteAsync(id);
            return Ok(new { message = "Đã chuyển trạng thái user sang ngừng hoạt động" });
        }
    }
}
