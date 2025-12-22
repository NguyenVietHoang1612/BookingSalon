using BookingSalon.Areas.Admin.Models;
using BookingSalon.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookingSalon.Services.Interface
{
    public interface IUsersService
    {
        Task<IEnumerable<Users>> GetAllAsync();
        Task<Users?> GetByIdAsync(string id);
        Task<IdentityResult> CreateUserAsync(AccountUserViewModel user);
        Task<IdentityResult> UpdateUserAsync(AccountUserViewModel user);
        Task SoftDeleteAsync(string id);
    }
}
