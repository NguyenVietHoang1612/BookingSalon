using BookingSalon.Areas.Admin.Models;
using BookingSalon.Migrations;
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
    public class StylistProfileController : Controller
    {
        private readonly IStylistProfileService _stylistProfileService;
        private readonly UserManager<Users> _userManager;

        public StylistProfileController(IStylistProfileService stylistProfileService, UserManager<Users> userManager)
        {
            _stylistProfileService = stylistProfileService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string term)
        {
            var stylistProfiles = await _stylistProfileService.GetAllAsync();

            if (!string.IsNullOrEmpty(term))
            {
                stylistProfiles = stylistProfiles.Where(f => f.StylistSkill.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
                ViewBag.SearchTerm = term;
            }

            return View(stylistProfiles);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var stylists = await _userManager.GetUsersInRoleAsync("Stylist");

            var selectlistSylist = new SelectList(stylists, "Id", "FullName");
            var branches = await _stylistProfileService.GetAllBranchAsync();
            var selectlistBranch = new SelectList(branches, "BranchId", "Branch_Name");

            var vm = new CreateStylistProfileVM
            {
                StylistUsersSelectList = selectlistSylist,
                BranchSelectList = selectlistBranch

            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStylistProfileVM stylistProfileVM)
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

            var result = await _stylistProfileService.CreateAsync(stylistProfileVM);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.Errors;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(stylistProfileVM);

            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var profile = await _stylistProfileService.GetByIdAsync(id);

            if (profile == null) return NotFound();

            var branches = await _stylistProfileService.GetAllBranchAsync();
            var selectlistBranch = new SelectList(branches, "BranchId", "Branch_Name");

            var vm = new UpdateStylistProfileVM
            {
                StylistProfile = profile.Data,

                BranchSelectList = selectlistBranch,
                Images = profile.Data.StylistImages.Select(img => new StylistImageItemVM
                {
                    ImageId = img.ImageId,
                    ImageUrl = img.ImageUrl,
                    IsRemoved = false
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, UpdateStylistProfileVM stylistProfileVM)
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

            var result = await _stylistProfileService.UpdateAsync(id, stylistProfileVM);

            if (!result.Succeeded)
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View(stylistProfileVM);

            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            await _stylistProfileService.DeleteAsync(id);
            return Ok(new { message = "Đã chuyển trạng thái user sang ngừng hoạt động" });
        }
    }
}
