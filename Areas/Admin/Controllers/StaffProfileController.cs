using BookingSalon.Areas.Admin.Models;
using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Models.Entities;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StaffProfileController : Controller
    {
        private readonly IStaffProfileService _staffProfileService;
        private readonly IBranchService _branchService;
        private readonly IAddressService _addressService;
        private readonly IUsersService _usersService;

        public StaffProfileController(IStaffProfileService stafftProfileService, IBranchService branchService, 
            IAddressService addressService, IUsersService usersService)
        {
            _staffProfileService = stafftProfileService;
            _branchService = branchService;
            _addressService = addressService;
            _usersService = usersService;
        }

        public async Task<IActionResult> Index(int? pageNumber, string term)
        {
             int pageSize = 10;
            var listService = await _staffProfileService.GetPagedListAsync(pageNumber ?? 1, pageSize, term);


            if (!string.IsNullOrEmpty(term))
            {
                ViewBag.SearchTerm = term;
            }

            return View(listService);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var branches = await _branchService.GetAllBranchActiveAsync();
            var selectlistBranch = new SelectList(branches, "BranchId", "Branch_Name");
            var role = await _staffProfileService.GetRoleStaff();
            var selectlistRole = new SelectList(role, "Id", "Name");
            var provinces = await _addressService.GetAllProvinceAsync();
            ViewBag.Provinces = new SelectList(provinces, "Id", "Name");

            var vm = new StaffCombinedCreateVM
            {
                BranchSelectList = selectlistBranch,
                RoleSelectList = selectlistRole,

            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StaffCombinedCreateVM viewModel)
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
                TempData["Warning"] = $"Lỗi Bind các dữ liệu profile: " + errorMessage;

                var provinces = await _addressService.GetAllProvinceAsync();
                ViewBag.Provinces = new SelectList(provinces, "Id", "Name");

                var branches = await _branchService.GetAllBranchActiveAsync();
                viewModel.BranchSelectList = new SelectList(branches, "BranchId", "Branch_Name");

                var roles = await _staffProfileService.GetRoleStaff();
                viewModel.RoleSelectList = new SelectList(roles, "Id", "Name");
                await PopulateAddressDropdowns(viewModel.User);
                return View(viewModel);
            }

            var result = await _staffProfileService.CreateAsync(viewModel);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                TempData["Error"] = "Thêm profile thất bại";
                var provinces = await _addressService.GetAllProvinceAsync();
                ViewBag.Provinces = new SelectList(provinces, "Id", "Name");

                var branches = await _branchService.GetAllBranchActiveAsync();
                viewModel.BranchSelectList = new SelectList(branches, "BranchId", "Branch_Name");

                var roles = await _staffProfileService.GetRoleStaff();
                viewModel.RoleSelectList = new SelectList(roles, "Id", "Name");
                await PopulateAddressDropdowns(viewModel.User);
                return View(viewModel);

            }
            TempData["Success"] = $"Thêm profile {viewModel.User.FullName} thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var profile = await _staffProfileService.GetByIdAsync(id);

            if (profile == null) return NotFound();

            var user = await _usersService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var branches = await _branchService.GetAllBranchActiveAsync();
            var selectlistBranch = new SelectList(branches, "BranchId", "Branch_Name");
            var role = await _staffProfileService.GetRoleStaff();
            var selectlistRole = new SelectList(role, "Id", "Name");

            var vm = new StaffCombinedUpdateVM();
            var ward = await _addressService.GetAddressByWardIdAsync(user.WardId ?? 0);

            vm = new StaffCombinedUpdateVM
            {
                User = profile.Data.Staff,
                ProfileInfo = profile.Data,
                ProvinceId = ward?.District.ProvinceId,
                DistrictId = ward?.DistrictId,
                BranchSelectList = selectlistBranch,
                RoleSelectList = selectlistRole
                    
            };

            ViewBag.SelectedProvinceId = vm.ProvinceId;
            ViewBag.SelectedDistrictId = vm.DistrictId;
            
            var provinces = await _addressService.GetAllProvinceAsync();
            ViewBag.Provinces = new SelectList(provinces, "Id", "Name");

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, StaffCombinedUpdateVM viewModel)
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
                TempData["Warning"] = $"Lỗi Bind các dữ liệu profile: " + errorMessage;

                var branches = await _branchService.GetAllBranchActiveAsync();
                viewModel.BranchSelectList = new SelectList(branches, "BranchId", "Branch_Name");

                var roles = await _staffProfileService.GetRoleStaff();
                viewModel.RoleSelectList = new SelectList(roles, "Id", "Name");
                return View(viewModel);
            }

            var result = await _staffProfileService.UpdateAsync(id, viewModel);

            if (!result.Succeeded)
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                TempData["Error"] = "Update profile thất bại";

                return View(viewModel);

            }
            TempData["Success"] = $"Update profile {viewModel.User.FullName} thành công!";
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

        private async Task PopulateAddressDropdowns(UsersModel users)
        {
            var branches = await _branchService.GetAllBranchActiveAsync();
            var selectlistBranch = new SelectList(branches, "BranchId", "Branch_Name");
            var provinces = await _addressService.GetAllProvinceAsync();
            ViewBag.Provinces = new SelectList(provinces, "Id", "Name");

            if (users.WardId > 0)
            {
                var ward = await _usersService.GetWardById(users);

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

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result =  await _staffProfileService.DeleteAsync(id);
            if (result.Succeeded)
            {
                return Ok(new { message = "Xóa nhân viên thành công" });
            }

            return BadRequest(new { success = false, message = "Không thể xóa dữ liệu" });
        }
    }
}
