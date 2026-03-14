using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel;

namespace BookingSalon.Services.Interface
{
    public interface ICouponService
    {
        Task<ServiceResult<IEnumerable<CouponModel>>> GetAllAsync();
        Task<PaginatedList<CouponModel>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            string searchTerm);

        Task<ServiceResult<CouponModel>> GetByIdAsync(int id);
        Task<ServiceResult<CouponModel>> CreateAsync(CouponModel model);
        Task<ServiceResult<CouponModel>> UpdateAsync(int id, CouponModel model);
        Task<ServiceResult<CouponModel>> DeleteAsync(int id);

        Task<ServiceResult<CouponValidateViewModel>>
            ValidateForBookingAsync(
                string code,
                decimal orderAmount,
                string userId);
    }
}
