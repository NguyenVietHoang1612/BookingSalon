using BookingSalon.Models.Entities;

namespace BookingSalon.Services.Interface
{
    public interface IFixedTimeSlotService
    {
        Task<IEnumerable<FixedTimeSlot>> GetAllListAsync();
        Task<ServiceResult<FixedTimeSlot>> GetByIdAsync(int id);
        Task<ServiceResult<FixedTimeSlot>> CreateAsync(FixedTimeSlot fixedTime);
        Task<ServiceResult<FixedTimeSlot>> UpdateAsync(int id, FixedTimeSlot fixedTime);
        Task<ServiceResult<FixedTimeSlot>> DeleteAsync(int id);
    }
}
