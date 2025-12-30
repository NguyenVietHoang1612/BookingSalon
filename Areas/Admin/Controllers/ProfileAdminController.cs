using BookingSalon.Areas.Admin.Models;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProfileAdminController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly IFileService _fileService;

        public ProfileAdminController(
            UserManager<Users> userManager,
            IFileService fileService)
        {
            _userManager = userManager;
            _fileService = fileService;
        }

        public async Task<IActionResult> Update()
        {
            var user = await _userManager.GetUserAsync(User);

            UserUpdateViewModel vm = new UserUpdateViewModel
            {
                User = user
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserUpdateViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

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

            user.FullName = vm.User.FullName;
            user.Email = vm.User.Email;
            user.UserName = vm.User.Email;
            user.PhoneNumber = vm.User.PhoneNumber;
            user.RoleId = user.RoleId;
            user.Status = true;
            user.EmailConfirmed = true;
            user.Update_At = DateTime.Now;

            if (vm.User.Avatar_Image_Upload != null)
            {
                if (!string.IsNullOrEmpty(user.Avatar_Name))
                {
                    await _fileService.DeleteFileAsync(user.Avatar_Name, "users");
                }

                user.Avatar_Name =
                    await _fileService.UploadFileAsync(vm.User.Avatar_Image_Upload, "users");
            }

            if (!string.IsNullOrWhiteSpace(vm.Password))
            {
                await _userManager.RemovePasswordAsync(user);
                var passResult = await _userManager.AddPasswordAsync(user, vm.Password);

                if (!passResult.Succeeded)
                {
                    foreach (var error in passResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(vm);
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Cập nhật thông tin thành công!";
                return RedirectToAction(nameof(Update));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(vm);
        }
        
    }
}
