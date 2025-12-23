using BookingSalon.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookingSalon.Services.Interface
{
    public interface IBranchService
    {
        Task<ServiceResult<IEnumerable<Branch>>> GetAllAsync();
        Task<ServiceResult<Branch>> GetByIdAsync(int id);
        Task<ServiceResult<Branch>> CreateAsync(Branch branch);
        Task<ServiceResult<Branch>> UpdateAsync(int id, Branch branch);
        Task<ServiceResult<Branch>> DeleteAsync(int id);
    }
}
