using BookingSalon.Areas.Admin.Models;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRepository<Users> _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public UsersService(UserManager<Users> userManager, RoleManager<IdentityRole> roleManager,IRepository<Users> repository, IWebHostEnvironment webHostEnvironment, IFileService fileService)
        {
            _userManager = userManager;
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }

        public Task<Users?> GetByIdAsync(string id)
        {
            var user = _userManager.FindByIdAsync(id);
            return user;
        }

        public async Task<IdentityResult> CreateUserAsync(AccountUserViewModel userVM)
        {
            if (userVM == null || string.IsNullOrEmpty(userVM.User.Email))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Không được để trống. Vui lòng nhập đầy đủ" });
            }

            var existingUser = await _userManager.FindByEmailAsync(userVM.User.Email);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email đã được sử dụng" });
            }

            userVM.User.UserName = userVM.User.Email;
            userVM.User.Create_At = DateTime.Now;
            userVM.User.Update_At = DateTime.Now;
            userVM.User.Status = true;

            if (userVM.User.Avatar_Image_Upload != null)
            {
                userVM.User.Avatar_Name = await _fileService
                    .UploadFileAsync(userVM.User.Avatar_Image_Upload, "users");
            }

            var result = await _userManager.CreateAsync(userVM.User, userVM.Password);

            if (result.Succeeded)
            {
                var role = await _roleManager.FindByIdAsync(userVM.User.RoleId);
                result = await _userManager.AddToRoleAsync(userVM.User, role.Name);
            }
            else
            {     
                 return IdentityResult.Failed(new IdentityError { Description = "Lỗi khi tạo người dùng" });
            }

            return result;
        }

        public async Task<IdentityResult> UpdateUserAsync(AccountUserViewModel userVM)
        {
            var existingUser = await _userManager.FindByIdAsync(userVM.User.Id);

            if (existingUser == null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "Không tìm thấy người dùng" }
                );
            }

            existingUser.FullName = userVM.User.FullName;
            existingUser.PhoneNumber = userVM.User.PhoneNumber;
            existingUser.Update_At = DateTime.Now;
            if (userVM.User.Avatar_Image_Upload != null)
            {
                existingUser.Avatar_Name = await _fileService.UploadFileAsync(userVM.User.Avatar_Image_Upload, "users");
            }
            return await _userManager.UpdateAsync(existingUser);
        }

        public async Task SoftDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Status = false;
                user.Update_At = DateTime.Now;
                await _userManager.UpdateAsync(user);
            }
        }
    }
}
