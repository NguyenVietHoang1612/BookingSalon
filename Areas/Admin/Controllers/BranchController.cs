using BookingSalon.Areas.Admin.Models;
using BookingSalon.Data;
using BookingSalon.Data.Repository; 
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BranchController : Controller
    {
        private readonly IBranchService _branchService;
        private readonly IAddressService _addressService;

        public BranchController(IBranchService branchService, IAddressService addressService)
        {
            _branchService = branchService;
            _addressService = addressService;
        }

        public async Task<IActionResult> Index(int? pageNumber, string term)
        {
            int pageSize = 10;
            var pagedData = await _branchService.GetPagedListAsync(pageNumber ?? 1, pageSize, term);

            if (!string.IsNullOrEmpty(term))
            {
                ViewBag.SearchTerm = term;
            }

            return View(pagedData);
        }

        public async Task<IActionResult> Create()
        {
            var provinces = await _addressService.GetAllProvinceAsync();
            ViewBag.Provinces = new SelectList(provinces, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BranchModel branch)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = $"Lỗi Bind dữ liệu hạng thành viên!";
                await PopulateAddressDropdowns(branch); 
                return View(branch);
            }

            var result = await _branchService.CreateAsync(branch);
            if (!result.Succeeded)
            {
                TempData["Error"] = $"Thêm hạng thành viên {branch.Branch_Name} thất bại: " + result.Errors;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                await PopulateAddressDropdowns(branch);
                return View(branch);
            }
            TempData["Success"] = "Thêm hạng thành viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var result = await _branchService.GetByIdAsync(id);
            if (!result.Succeeded || result.Data == null) return NotFound();

            var branch = result.Data;
            var provinces = await _addressService.GetAllProvinceAsync();
            ViewBag.Provinces = new SelectList(provinces, "Id", "Name");
            await PopulateAddressDropdowns(branch);

            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, BranchModel branch)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = $"Lỗi Bind dữ liệu hạng thành viên!";

                await PopulateAddressDropdowns(branch);
                return View(branch);
            }

            var result = await _branchService.UpdateAsync(id, branch);
            if (!result.Succeeded)
            {
                TempData["Error"] = $"Cập nhật hạng thành viên {branch.Branch_Name} thất bại: " + result.Errors;

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                await PopulateAddressDropdowns(branch);
                return View(branch);
            }
            TempData["Success"] = "Thêm hạng thành viên thành công!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetDistricts(int provinceId)
        {
            var districts = await _addressService.GetAllDistrictByProvinceIdAsync(provinceId);
            return Json(districts);
        }

        [HttpGet]
        public async Task<JsonResult> GetWards(int districtId)
        {
            var wards = await _addressService.GetAllWardByDistrictIdAsync(districtId);
            return Json(wards);
        }

        private async Task PopulateAddressDropdowns(BranchModel branch)
        {
            var provinces = await _addressService.GetAllProvinceAsync();
            ViewBag.Provinces = new SelectList(provinces, "Id", "Name");

            if (branch.WardId > 0)
            {
                var ward = await _branchService.GetWardById(branch);

                if (ward != null)
                {
                    var districts = await _addressService.GetAllDistrictByProvinceIdAsync(ward.District.ProvinceId);

                    var wards = await _addressService.GetAllWardByDistrictIdAsync(ward.DistrictId);

                    ViewBag.Districts = new SelectList(districts, "Id", "Name", ward.DistrictId);
                    ViewBag.Wards = new SelectList(wards, "Id", "Name", ward.Id);
                    ViewBag.SelectedProvinceId = ward.District.ProvinceId;
                }
            }
        }

        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _branchService.DeleteAsync(id);

            if (result.Succeeded)
            {
                return Ok(new { success = true, message = "Xóa thành công" });
            }

            return BadRequest(new { success = false, message = "Không thể xóa dữ liệu" });
        }
    }
}