using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;

namespace BookingSalon.Services.Interface
{
    public interface INewsService
    {
        Task<IEnumerable<NewsModel>> GetAllAsync();
        Task<ServiceResult<NewsModel>> GetByIdAsync(int id);
        Task<ServiceResult<NewsVM>> CreateAsync(NewsVM newsVM, string userId);
        Task<ServiceResult<NewsVM>> UpdateAsync(int id, NewsVM newsVM);
        Task<ServiceResult<NewsModel>> DeleteAsync(int id);
        Task<PaginatedList<NewsModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm);
        Task<PaginatedList<NewsModel>> GetReceptionPagedListAsync(string receptionId, int pageNumber, int pageSize, string searchTerm);
    }
}
