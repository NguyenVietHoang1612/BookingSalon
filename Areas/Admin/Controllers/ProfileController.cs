using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProfileController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly IFileService _fileService;

        public ProfileController(
            UserManager<Users> userManager,
            IFileService fileService)
        {
            _userManager = userManager;
            _fileService = fileService;
        }

        public async Task<IActionResult> Update()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Users model)
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

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.RoleId = user.RoleId;
            user.PhoneNumber = model.PhoneNumber;
            user.EmailConfirmed = model.EmailConfirmed;
            user.Status = user.Status;
            user.Update_At = DateTime.Now;

            if (model.Avatar_Image_Upload != null)
            {
                if (!string.IsNullOrEmpty(user.Avatar_Name))
                {
                    await _fileService.DeleteFileAsync(user.Avatar_Name, "users");
                }

                user.Avatar_Name =
                    await _fileService.UploadFileAsync(model.Avatar_Image_Upload, "users");
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Update));
        }
        
    }
}
