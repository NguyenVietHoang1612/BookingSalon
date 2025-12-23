using BookingSalon.Areas.Admin.Models;
using BookingSalon.Data;
using BookingSalon.Models.Entities;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BranchController : Controller
    {
        private readonly IBranchService _branchService;


        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        public async Task<IActionResult> Index()
        {
            var listBranchesResult = await _branchService.GetAllAsync();

            if (!listBranchesResult.Succeeded)
            {
                TempData["ErrorMessage"] = listBranchesResult.Errors;
                foreach(var error in listBranchesResult.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View();
            }
            return View(listBranchesResult.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Branch branch)
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

            var result = await _branchService.CreateAsync(branch);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.Errors;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(branch);

            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            var branch = await _branchService.GetByIdAsync(id);

            if (branch == null) return NotFound();

            Branch branchDetail = new Branch
            {
                Branch_Name = branch.Data.Branch_Name,
                Address = branch.Data.Address,
                Phone = branch.Data.Phone,
                Status = branch.Data.Status
            };

            return View(branchDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Branch branch)
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

            var result = await _branchService.UpdateAsync(id, branch);

            if (!result.Succeeded)
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View(branch);

            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SoftDelete(int id)
        {
            await _branchService.DeleteAsync(id);
            return Ok(new { message = "Đã chuyển trạng thái user sang ngừng hoạt động" });
        }
    }
}
