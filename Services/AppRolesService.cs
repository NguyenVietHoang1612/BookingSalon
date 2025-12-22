using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class AppRolesService : IAppRolesService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AppRolesService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<IdentityRole?>> GetAllAsync()
        {
            if (_roleManager == null)
            {
                throw new Exception("RoleManager chưa được khởi tạo!");
            }

            var roles = await _roleManager.Roles.ToListAsync();
            return roles ?? new List<IdentityRole>();
        }
        public async Task<IdentityRole?> GetByIdAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return role;
        }

        public async Task<IdentityResult> CreateUserAsync(IdentityRole role)
        {
            if (role == null || string.IsNullOrEmpty(role.Name))
            {
                return IdentityResult.Failed(new IdentityError { Description = "trường Role không được để trống" });
            }

            var existingRole = await _roleManager.FindByNameAsync(role.Name);
            if (existingRole != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role đã tồn tại" });
            }

            return await _roleManager.CreateAsync(role);
        }
        public async Task<IdentityResult> UpdateUserAsync(IdentityRole role)
        {
            var existingRole = await _roleManager.FindByIdAsync(role.Id);

            if (existingRole == null) return IdentityResult.Failed(new IdentityError { Description = "Không tìm thấy Role" });

            existingRole.Name = role.Name;
            existingRole.NormalizedName = role.Name.ToUpper();

            return await _roleManager.UpdateAsync(existingRole);
        }

        public async Task<IdentityResult> DeleteAsync(IdentityRole role)
        {
            var result = await _roleManager.FindByIdAsync(role.Id);
            if (result == null) return IdentityResult.Failed(new IdentityError { Description = "Không tìm thấy Role để xóa" });

            return await _roleManager.DeleteAsync(result);
        }
    }
}
