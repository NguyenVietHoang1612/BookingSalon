using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Twilio.TwiML.Messaging;

namespace BookingSalon.Controllers
{

    public class AccountController : Controller
    {
        private readonly SignInManager<UsersModel> signInManager;
        private readonly UserManager<UsersModel> _userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUsersService _userService;
        private readonly ISmsSender _smsSender;

        public AccountController(SignInManager<UsersModel> signInManager, UserManager<UsersModel> userManager, RoleManager<IdentityRole> roleManager, IUsersService usersService, ISmsSender smsSender)
        {
            this.signInManager = signInManager;
            this._userManager = userManager;
            this.roleManager = roleManager;
            _userService = usersService;
            _smsSender = smsSender;
        }

        public IActionResult LoginInternal()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginInternal(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userLogin = await _userManager.FindByEmailAsync(model.Email);

                if (userLogin == null)
                {
                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                    return View(model);
                }

                if (await _userManager.IsInRoleAsync(userLogin, "Customer"))
                {
                    ModelState.AddModelError("", "Tài khoản khách hàng không thể đăng nhập tại đây.");
                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!userLogin.Status)
                    {
                        await signInManager.SignOutAsync(); 
                        ModelState.AddModelError("", "Tài khoản đã bị cấm.");
                        return View(model);
                    }

                    if (await _userManager.IsInRoleAsync(userLogin, "Admin"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }

                    return RedirectToAction("StaffUpdate", "Profile", new { area = "Admin" });
                }
                else
                {
                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Home", "Home");
        }

        public async Task<IActionResult> LogoutInternal()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("LoginInternal", "Account");
        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendOtp(AuthOtpViewModel model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber))
            {
                ModelState.AddModelError("", "Vui lòng nhập số điện thoại.");
                return View("Login", model);
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    ModelState.AddModelError("", "Số điện thoại này thuộc về nhân viên. Vui lòng đăng nhập trang nội bộ.");
                    return View("Login", model);
                }
            }
            else
            {
                var role = await roleManager.FindByNameAsync("Customer");
                var userVM = new UserCreateViewModel
                {
                    User = new UsersModel
                    {
                        UserName = model.PhoneNumber,
                        PhoneNumber = model.PhoneNumber,
                        FullName = "Khách hàng " + model.PhoneNumber,
                        RoleId = role?.Id ?? "",
                        Email = model.PhoneNumber + "@temp.com", 
                        Status = true
                    },
                    CustomerRank = new CustomerRankModel
                    {
                        CurrentPoints = 0,
                        LifetimePoints = 0,
                        Is_Active = true
                    }
                };

                var createResult = await _userService.CreateUserCustomerAsync(userVM);

                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("Login", model);
                }
                TempData["IsNewUser"] = true;
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            }

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");
            //string message = $"Ma OTP cua ban la: {code}. Vui long khong chia se cho bat ky ai.";
            //await _smsSender.SendSmsAsync(model.PhoneNumber, message);

            TempData["OtpSent"] = "Mã OTP đã được gửi đến số điện thoại: ";
            System.Diagnostics.Debug.WriteLine($"SĐT: {model.PhoneNumber} - MÃ OTP LÀ: {code}");

            

            return View("VerifyOtp", new AuthOtpViewModel { PhoneNumber = model.PhoneNumber });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(AuthOtpViewModel model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (user == null) return RedirectToAction("Login");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", model.OtpCode);

            if (isValid)
            {
                user.PhoneNumberConfirmed = true;
                await _userManager.UpdateAsync(user);

                await signInManager.SignInAsync(user, isPersistent: true);

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                if (TempData["IsNewUser"] != null && (bool)TempData["IsNewUser"])
                {
                    return RedirectToAction("Index", "Profile");
                }

                return RedirectToAction("Home", "Home");
            }

            ModelState.AddModelError("", "Mã OTP không chính xác hoặc đã hết hạn.");
            return View(model);
        }
    }
}
