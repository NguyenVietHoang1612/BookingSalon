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
        private readonly IFileService _fileService;

        public UsersService(UserManager<Users> userManager, RoleManager<IdentityRole> roleManager, IFileService fileService)
        {
            _userManager = userManager;
            _fileService = fileService;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            var adminRoleId = adminRole?.Id;

            return await _userManager.Users
                .Where(u => u.RoleId != adminRoleId)
                .ToListAsync();
        }

        public Task<Users?> GetByIdAsync(string id)
        {
            var user = _userManager.FindByIdAsync(id);
            return user;
        }

        public async Task<IdentityResult> CreateUserAsync(UserCreateViewModel userVM)
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

        public async Task<IdentityResult> UpdateUserAsync(UserUpdateViewModel userVM, string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id);

            if (existingUser == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Không tìm thấy người dùng" });
            }

            existingUser.FullName = userVM.User.FullName;
            existingUser.Email = userVM.User.Email;
            existingUser.UserName = userVM.User.Email;
            existingUser.Address = userVM.User.Address;
            existingUser.RoleId = userVM.User.RoleId;
            existingUser.PhoneNumber = userVM.User.PhoneNumber;
            existingUser.EmailConfirmed = userVM.User.EmailConfirmed;
            existingUser.Status = userVM.User.Status; 
            existingUser.Update_At = DateTime.Now;

            if (userVM.User.Avatar_Image_Upload != null)
            {
                if (!string.IsNullOrEmpty(existingUser.Avatar_Name))
                {
                    await _fileService.DeleteFileAsync(existingUser.Avatar_Name, "users");
                }

                existingUser.Avatar_Name =
                    await _fileService.UploadFileAsync(userVM.User.Avatar_Image_Upload, "users");
            }

            if (!string.IsNullOrWhiteSpace(userVM.Password))
            {
                await _userManager.RemovePasswordAsync(existingUser);
                var passResult = await _userManager.AddPasswordAsync(existingUser, userVM.Password);

                if (!passResult.Succeeded)
                    return passResult;
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

        public async Task<PaginatedList<UserViewModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            var adminRoleId = adminRole?.Id;

            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(u => u.FullName.ToLower().Contains(searchTerm) || u.Email.ToLower().Contains(searchTerm) || u.Id.ToLower().Contains(searchTerm));
            }

            query = query.Where(u => u.RoleId != adminRoleId);

            query = query.OrderByDescending(u => u.Create_At);

            var viewmodelQuery = query.Select(u => new UserViewModel
            {
                User = u,
                RoleName = _roleManager.Roles
                    .Where(r => r.Id == u.RoleId)
                    .Select(r => r.Name)
                    .FirstOrDefault() ?? "N/A"
            });

            return await PaginatedList<UserViewModel>.CreateAsync(viewmodelQuery, pageNumber, pageSize);
        }
    }
}
