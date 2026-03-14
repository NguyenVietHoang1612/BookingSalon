using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Models.Entities;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Stylist,Skinner,Reception")]
    public class ProfileController : Controller
    {
        private readonly UserManager<UsersModel> _userManager;
        private readonly IStaffProfileService _staffService;
        private readonly IWorkScheduleService _workScheduleService;
        private readonly IBranchService _branchService;
        private readonly IUsersService _usersService;
        private readonly IAddressService _addressService;
        private readonly IStaffProfileService _staffProfileService;

        public ProfileController(UserManager<UsersModel> userManager, IStaffProfileService staffService, IWorkScheduleService workScheduleService, 
            IBranchService branchService, IUsersService usersService, IAddressService addressService, IStaffProfileService staffProfileService)
        {
            _userManager = userManager;
            _staffService = staffService;
            _workScheduleService = workScheduleService;
            _branchService = branchService;
            _usersService = usersService;
            _addressService = addressService;
            _staffProfileService = staffProfileService;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return NotFound();
            var vm = new UserViewModel();
            if (user.WardId != null)
            {
                var ward = _addressService.GetAddressByWardIdAsync(user.WardId.Value);

                vm = new UserViewModel
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    WardId = user.WardId,
                    ProvinceId = ward.Result.District.ProvinceId,
                    DistrictId = ward.Result.DistrictId,
                    Address = user.Address,
                    avatarName = user.Avatar_Name
                };

                ViewBag.SelectedProvinceId = vm.ProvinceId;
                ViewBag.SelectedDistrictId = vm.DistrictId;
            }
            else
            {
                vm = new UserViewModel
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    WardId = user.WardId,
                    Address = user.Address,
                    avatarName = user.Avatar_Name
                };
            }

            var provinces = await _addressService.GetAllProvinceAsync();
            ViewBag.Provinces = new SelectList(provinces, "Id", "Name");

            await PopulateAddressDropdowns(user);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserViewModel userViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            var id = user.Id;

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

                return View(userViewModel);
            }

            var result = await _usersService.UpdateUserAsync(userViewModel, id);

            if (!result.Succeeded)
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                TempData["Error"] = "Update profile thất bại: " + result.Errors;

                return View(userViewModel);

            }
            TempData["Success"] = $"Update profile {userViewModel.FullName} thành công!";
            return RedirectToAction(nameof(Update));
        }


        [Authorize(Roles = "Stylist,Skinner,Reception")]
        public async Task<IActionResult> StaffUpdate()
        {
            var user = await _userManager.GetUserAsync(User);

            var profile = await _staffService.GetByIdAsync(user.Id);

            if (profile == null) return NotFound();

            var branches = await _branchService.GetAllBranchActiveAsync();;
            var role = await _staffService.GetRoleStaff();
            var selectlistRole = new SelectList(role, "Id", "Name");

            var vm = new StaffSelfUpdateVM();


            if (user.WardId != null)
            {
                var ward = await _addressService.GetAddressByWardIdAsync(user.WardId.Value);
                vm = new StaffSelfUpdateVM
                {
                    User = profile.Data.Staff,
                    StaffId = profile.Data.StaffId,
                    Branch_Id = profile.Data.Branch_Id,
                    Start_Work_Time = profile.Data.Start_Work_Time,
                    End_Work_Time = profile.Data.End_Work_Time,
                    Branch_name = profile.Data.Branch.Branch_Name,
                    ProvinceId = ward.District.ProvinceId,
                    DistrictId = ward.DistrictId,
                    Staff_Bio = profile.Data.Staff_Bio
                };
            }
            else
            {
                vm = new StaffSelfUpdateVM
                {
                    User = profile.Data.Staff,
                    StaffId = profile.Data.StaffId,
                    Branch_Id = profile.Data.Branch_Id,
                    Start_Work_Time = profile.Data.Start_Work_Time,
                    End_Work_Time = profile.Data.End_Work_Time,
                    Branch_name = profile.Data.Branch.Branch_Name,
                    Staff_Bio = profile.Data.Staff_Bio
                };
            }

            

            ViewBag.SelectedProvinceId = vm.ProvinceId;
            ViewBag.SelectedDistrictId = vm.DistrictId;
    
            var provinces = await _addressService.GetAllProvinceAsync();
            ViewBag.Provinces = new SelectList(provinces, "Id", "Name");

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StaffUpdate(StaffSelfUpdateVM viewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            var id = user.Id;

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

                return View(viewModel);
            }

            var result = await _staffProfileService.StaffSelfUpdateAsync(id, viewModel);

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
            return RedirectToAction(nameof(StaffUpdate));
        }


        public IActionResult WorkSchedule()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Stylist, Skinner")]
        public async Task<JsonResult> GetStylistWorkSchedule(string stylistId)
        {
            var events = await _workScheduleService.GetWorkSchedule(stylistId);
            var busyEvents = events.Where(e => e.ExtendedProps.Status == false).ToList();
            return Json(events);
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
    }
}
