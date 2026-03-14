using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;

namespace BookingSalon.Services.Interface
{
    public interface ITypeOfServiceService
    {
        Task<IEnumerable<TypeOfServiceModel>> GetAllTypeServiceAsync();
        Task<ServiceResult<TypeOfServiceModel>> GetByIdAsync(int id);
        Task<ServiceResult<TypeOfServiceModel>> CreateAsync(TypeOfServiceModel fixedTime);
        Task<ServiceResult<TypeOfServiceModel>> UpdateAsync(int id, TypeOfServiceModel fixedTime);
        Task<ServiceResult<TypeOfServiceModel>> DeleteAsync(int id);
        Task<PaginatedList<TypeOfServiceModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm);
    }
}
