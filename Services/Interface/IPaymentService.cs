using BookingSalon.Models.Entities;

namespace BookingSalon.Services.Interface
{
    public interface IPaymentService
    {
        Task<ServiceResult<IEnumerable<PaymentModel>>> GetAllAsync();
        Task<ServiceResult<PaymentModel>> GetByIdAsync(Guid id);
        Task<ServiceResult<PaymentModel>> GetByBookingIdAsync(int id);
        Task<ServiceResult<PaymentModel>> CreatePaymentAsync(PaymentModel payment);
    }
}
