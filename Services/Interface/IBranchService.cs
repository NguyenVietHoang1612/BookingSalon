using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookingSalon.Services.Interface
{
    public interface IBranchService
    {
        Task<ServiceResult<IEnumerable<BranchModel>>> GetAllAsync();
        Task<IEnumerable<BranchModel>> GetAllBranchActiveAsync();
        Task<ServiceResult<BranchModel>> GetByIdAsync(int id);
        Task<WardModel> GetWardById(BranchModel branch);
        Task<ServiceResult<BranchModel>> CreateAsync(BranchModel branch);
        Task<ServiceResult<BranchModel>> UpdateAsync(int id, BranchModel branch);
        Task<ServiceResult<BranchModel>> DeleteAsync(int id);
        Task<PaginatedList<BranchModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm);

    }
}
