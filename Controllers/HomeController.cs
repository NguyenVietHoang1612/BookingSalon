using BookingSalon.Models;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookingSalon.Controllers
{

    public class HomeController : Controller
    {
        private readonly ITypeOfServiceService _typeOfServiceSalon;
        private readonly IHomeService _homeService;

        private readonly IBranchService _branchService;
        private readonly IStaffProfileService _stylistProfileService;

        public HomeController( 
            IBranchService branchService, IStaffProfileService stylistProfileService, ITypeOfServiceService typeOfServiceService, IHomeService homeService)
        {
            _branchService = branchService;
            _stylistProfileService = stylistProfileService;
            _typeOfServiceSalon = typeOfServiceService;
            _homeService = homeService;
        }

        public async Task<IActionResult> Service()
        {
            var branches = await _branchService.GetAllAsync();
            var profileStylist = await _stylistProfileService.GetAllAsync();
            var typeOfService = await _typeOfServiceSalon.GetAllTypeServiceAsync();

            var homeVM = new BarberViewModel
            {
                Branches = branches.Data,
                Categories = typeOfService,
                FeaturedStylists = profileStylist
            };

            return View(homeVM);
        }

        public async Task<IActionResult> Home()
        {
            if (User.Identity.IsAuthenticated)
            {

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                else if (User.IsInRole("Stylist") || User.IsInRole("Skinner") || User.IsInRole("Reception"))
                {
                    return RedirectToAction("StaffUpdate", "Profile", new { area = "Admin" });
                }
            }

            
            var homeViewModel = await _homeService.GetAllStaffAsync();
            return View(homeViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
