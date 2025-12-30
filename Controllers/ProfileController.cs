using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookingSalon.Controllers
{
    [Authorize]
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

        // POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Users model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
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

            return RedirectToAction(nameof(Index));
        }
    }
}
