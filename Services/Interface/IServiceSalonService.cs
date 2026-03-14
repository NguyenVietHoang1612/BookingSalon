using BookingSalon.Areas.Admin.Models;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;

namespace BookingSalon.Services.Interface
{
    public interface IServicesSalonService
    {
        Task<IEnumerable<ServiceModel>> GetAllServiceAsync();
        Task<IEnumerable<ServiceModel>> GetAllServiceActiveAsync();
        Task<ServiceResult<ServiceModel>> GetByIdAsync(int id);
        Task<ServiceResult<ServiceModel>> CreateAsync(ServiceModel service);
        Task<ServiceResult<ServiceModel>> UpdateAsync(int id, ServiceModel service);
        Task<ServiceResult<ServiceModel>> DeleteAsync(int id);
        Task<PaginatedList<ServiceModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm);
    }
}
