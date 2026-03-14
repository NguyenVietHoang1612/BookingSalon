using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;

namespace BookingSalon.Services.Interface
{
    public interface IStaffPortfolioService
    {
        Task<ServiceResult<IEnumerable<StaffPortfolioModel>>> GetAllAsync();
        Task<ServiceResult<StaffPortfolioModel>> GetByIdAsync(int id);
        Task<ServiceResult<StaffPortfolioModel>> CreateAsync(StaffPortfolioModel model);
        Task<ServiceResult<StaffPortfolioModel>> UpdateAsync(int id, StaffPortfolioModel model);
        Task<ServiceResult<StaffPortfolioModel>> UpdateIsFeaturedAsync(int id, bool isFeatured);
        Task<ServiceResult<StaffPortfolioModel>> DeleteAsync(int id);
        Task<PaginatedList<StaffPortfolioModel>> GetStaffPagedListAsync(string staffId, int pageNumber, int pageSize, string searchTerm);
        Task<PaginatedList<StaffPortfolioModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm);
    }
}
