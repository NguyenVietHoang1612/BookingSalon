using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookingSalon.Services.Interface
{
    public interface IAppRolesService
    {
        Task<IEnumerable<IdentityRole?>> GetAllAsync();
        Task<IdentityRole?> GetByIdAsync(string id);
        Task<IdentityResult> CreateUserAsync(IdentityRole role);
        Task<IdentityResult> UpdateUserAsync(IdentityRole role);
        Task<IdentityResult> DeleteAsync(IdentityRole role);
        Task<PaginatedList<IdentityRole>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm);
    }
}
