using BookingSalon.Areas.Admin.Models;
using BookingSalon.Models.Entities;

namespace BookingSalon.Services.Interface
{
    public interface IServicesSalonService
    {
        Task<IEnumerable<Service>> GetAllServiceAsync();
        Task<IEnumerable<TypeOfService>> GetAllTypeOfServiceAsync();
        Task<ServiceResult<Service>> GetByIdAsync(int id);
        Task<ServiceResult<Service>> CreateAsync(Service service);
        Task<ServiceResult<Service>> UpdateAsync(int id, Service service);
        Task<ServiceResult<Service>> DeleteAsync(int id);
        
    }
}
