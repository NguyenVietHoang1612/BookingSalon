using BookingSalon.Models.Entities;

namespace BookingSalon.Services.Interface
{
    public interface ITypeOfServiceService
    {
        Task<IEnumerable<TypeOfService>> GetAllServiceAsync();
        Task<ServiceResult<TypeOfService>> GetByIdAsync(int id);
        Task<ServiceResult<TypeOfService>> CreateAsync(TypeOfService fixedTime);
        Task<ServiceResult<TypeOfService>> UpdateAsync(int id, TypeOfService fixedTime);
        Task<ServiceResult<TypeOfService>> DeleteAsync(int id);
    }
}
