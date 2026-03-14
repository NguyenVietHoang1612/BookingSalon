using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingSalon.Controllers
{
    [Authorize] 
    public class ProfileController : Controller
    {
        private readonly UserManager<UsersModel> _userManager;
        private readonly ICustomerRankService _customerRankService;
        private readonly IUsersService _usersService;
        private readonly IAddressService _addressService;
        private readonly IRankService _rankService;

        public ProfileController(
            UserManager<UsersModel> userManager,
            ICustomerRankService customerRankService,
            IUsersService usersService,
            IAddressService addressService,
            IRankService rankService)
        {
            _userManager = userManager;
            _customerRankService = customerRankService;
            _usersService = usersService;
            _addressService = addressService;
            _rankService = rankService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var customerRank = await _customerRankService.GetByCustomerIdAsync(user.Id);
            var rankList = await _rankService.GetAllAsync();

            var nextRank = rankList.Data
                .FirstOrDefault(r => r.MinPoint > customerRank.CurrentPoints);

            var currentPoints = customerRank?.CurrentPoints ?? 0;

            string? nextRankName = null;
            int? pointsToNextRank = null;

            if (nextRank != null)
            {
                nextRankName = nextRank.RankName;
                pointsToNextRank = nextRank.MinPoint - currentPoints;
            }

            var viewModel = new ProfileViewModel();
            if (user.WardId != null)
            {
                var ward = await _addressService.GetAddressByWardIdAsync(user.WardId.Value);

                viewModel = new ProfileViewModel
                {
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    WardId = user.WardId,
                    CurrentAvatar = user.Avatar_Name,
                    ProvinceId = ward.District.ProvinceId,
                    DistrictId = ward.DistrictId,
                    RankName = customerRank?.Rank?.RankName ?? "Chưa có hạng",
                    CurrentPoints = customerRank?.CurrentPoints ?? 0,
                    LifetimePoints = customerRank?.LifetimePoints ?? 0,
                    NextRankName = nextRankName,
                    PointsToNextRank = pointsToNextRank,
                    DiscountPercent = customerRank?.Rank?.DiscountPercent ?? 0
                };
                ViewBag.SelectedProvinceId = viewModel.ProvinceId;
                ViewBag.SelectedDistrictId = viewModel.DistrictId;
            }
            else
            {
                viewModel = new ProfileViewModel
                {
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    WardId = user.WardId,
                    CurrentAvatar = user.Avatar_Name,
                    RankName = customerRank?.Rank?.RankName ?? "Chưa có hạng",
                    CurrentPoints = customerRank?.CurrentPoints ?? 0,
                    LifetimePoints = customerRank?.LifetimePoints ?? 0,
                    DiscountPercent = customerRank?.Rank?.DiscountPercent ?? 0,
                    NextRankName = nextRankName,
                    PointsToNextRank = pointsToNextRank,
                };
            }

            var provinces = await _addressService.GetAllProvinceAsync();
            ViewBag.Provinces = new SelectList(provinces, "Id", "Name");

            await PopulateAddressDropdowns(user);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                var result = await _usersService.UpdateCustomerProfileAsync(user.Id, model);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
            }
            var rank = await _customerRankService.GetByCustomerIdAsync(user.Id);

            model.RankName = rank?.Rank?.RankName;
            model.CurrentPoints = rank?.CurrentPoints ?? 0;
            model.LifetimePoints = rank?.LifetimePoints ?? 0;
            model.CurrentAvatar = user.Avatar_Name;

            return View("Index", model);
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