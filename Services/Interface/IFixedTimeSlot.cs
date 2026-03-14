using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;

namespace BookingSalon.Services.Interface
{
    public interface IFixedTimeSlotService
    {
        Task<IEnumerable<FixedTimeSlotModel>> GetAllListAsync();
        Task<ServiceResult<FixedTimeSlotModel>> GetByIdAsync(int id);
        Task<ServiceResult<FixedTimeSlotModel>> CreateAsync(FixedTimeSlotModel fixedTime);
        Task<ServiceResult<FixedTimeSlotModel>> UpdateAsync(int id, FixedTimeSlotModel fixedTime);
        Task<ServiceResult<FixedTimeSlotModel>> DeleteAsync(int id);
        Task<PaginatedList<FixedTimeSlotModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm);
    }
}
