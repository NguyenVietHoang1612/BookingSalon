using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Models.Entities;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Reception")]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly ICustomerRankService _customerRankService;
        private readonly IAddressService _addressService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(IUsersService usersService, RoleManager<IdentityRole> roleManager, 
            ICustomerRankService customerRankService, IAddressService addressService)
        {
            _usersService = usersService;
            _roleManager = roleManager;
            _customerRankService = customerRankService;
            _addressService = addressService;
        }

        public async Task<IActionResult> Index(int? pageNumber, string term, string sortOrder)
        {
            int pageSize = 10;
            var listService = await _usersService.GetPagedListAsync(pageNumber ?? 1, pageSize, term, sortOrder);

            if (!string.IsNullOrEmpty(term))
            {
                ViewBag.SearchTerm = term;
            }

            if (!string.IsNullOrEmpty(sortOrder))
            {
                ViewBag.SortOrder = sortOrder;
            }

            return View(listService);
        }

        public async Task<IActionResult> Create()
        {
            var customerRole = await _roleManager.FindByNameAsync("Customer");

            if (customerRole == null)
            {
                TempData["Error"] = "Vai trò Customer chưa được tạo.";
                return RedirectToAction(nameof(Index));
            }
            var provinces = await _addressService.GetAllProvinceAsync();

            ViewBag.Provinces = new SelectList(provinces, "Id", "Name");

            var model = new UserCreateViewModel
            {
                CustomerRoleId = customerRole.Id,
            };

            return View(model);
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
                GetRoleCustomer();
                await PopulateAddressDropdowns(userVM.User);
                string errorMessage = string.Join("; ", errors);
                TempData["Warning"] = $"Lỗi Bind dữ liệu người dùng: " + errorMessage;

                return BadRequest(errorMessage);
            }

            var result = await _usersService.CreateUserCustomerAsync(userVM);

            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));

                TempData["Error"] = "Thêm người dùng thất bại: " + errorMessages;

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                GetRoleCustomer();
                await PopulateAddressDropdowns(userVM.User);
                return View(userVM);
                
            }

            TempData["Success"] = $"Thêm người dùng {userVM.User.FullName} thành công!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var user = await _usersService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var customerRank = await _customerRankService.GetByCustomerIdAsync(id);

            if (customerRank == null)
            {
                customerRank = new CustomerRankModel { Customer_Id = id };
            }

            var CustomerRole = await _roleManager.FindByNameAsync("Customer");

            UserUpdateViewModel accountUserViewModel = new UserUpdateViewModel
            {
                User = user,
                CustomerRank = customerRank, 
                CustomerRole = CustomerRole.Id
            };

            if (user.WardId == null)
            {
                ViewBag.SelectedProvinceId = null;
                ViewBag.SelectedDistrictId = null;
            }
            else
            {
                var ward = await _addressService.GetAddressByWardIdAsync(user.WardId.Value);
                accountUserViewModel.ProvinceId = ward.District.ProvinceId;
                accountUserViewModel.DistrictId = ward.DistrictId;

                ViewBag.SelectedProvinceId = accountUserViewModel.ProvinceId;
                ViewBag.SelectedDistrictId = accountUserViewModel.DistrictId;
            }

            var provinces = await _addressService.GetAllProvinceAsync();
            ViewBag.Provinces = new SelectList(provinces, "Id", "Name");

            await PopulateAddressDropdowns(user);
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
                await PopulateAddressDropdowns(userVM.User);
                string errorMessage = string.Join("; ", errors);
                TempData["Warning"] = $"Lỗi Bind dữ liệu người dùng: " + errorMessage;
                return View(userVM);
            }

            var result = await _usersService.UpdateUserCustomerAsync(userVM, id);

            if (!result.Succeeded)
            {
                TempData["Error"] = "Cập nhật người dùng thất bại: " + result.Errors;

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                await PopulateAddressDropdowns(userVM.User);
                return View(userVM);
            }
            TempData["Success"] = $"Cập nhật người dùng {userVM.User.FullName} thành công!";

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var result = await _usersService.Delete(id);

            if (result.Succeeded)
            {
                return Ok(new { message = "Xóa khách hàng thành công" });
            }

            return BadRequest(new { success = false, message = "Không thể xóa dữ liệu" });
        }

        public async Task GetRoleCustomer()
        {
            var CustomerRole = await _roleManager.FindByNameAsync("Customer");
            ViewBag.CustomerRole = CustomerRole;
        }
    }
}
