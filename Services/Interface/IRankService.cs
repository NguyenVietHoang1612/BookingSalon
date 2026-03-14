using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;

namespace BookingSalon.Services.Interface
{
    public interface IRankService
    {
        Task<ServiceResult<IEnumerable<RankModel>>> GetAllAsync();
        Task<ServiceResult<RankModel>> GetByIdAsync(int id);
        Task<ServiceResult<RankModel>> CreateAsync(RankModel rank);
        Task<ServiceResult<RankModel>> UpdateAsync(int id, RankModel rank);
        Task<ServiceResult<RankModel>> DeleteAsync(int id);
        Task<PaginatedList<RankModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm);
    }
}
