using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<PaymentModel>> CreatePaymentAsync(PaymentModel payment)
        {
            try
            {
                var repo = _unitOfWork.Repository<PaymentModel>();

                if (payment.PaymentId == Guid.Empty)
                    payment.PaymentId = Guid.NewGuid();

                payment.CreatedDate = DateTime.Now;
                if (payment.PaymentDate == null) payment.PaymentDate = DateTime.Now;

                payment.StaffId = null;
                payment.Booking = null;

                await repo.AddAsync(payment);

                var affectedRows = await _unitOfWork.SaveChangesAsync();

                if (affectedRows > 0)
                {
                    return ServiceResult<PaymentModel>.Success(payment);
                }

                return ServiceResult<PaymentModel>.Failed("Dữ liệu không có thay đổi nào được ghi nhận.");
            }
            catch (Exception ex)
            {
                var innerError = ex.InnerException?.Message ?? ex.Message;
                return ServiceResult<PaymentModel>.Failed($"Lỗi hệ thống: {innerError}");
            }
        }

        public async Task<ServiceResult<IEnumerable<PaymentModel>>> GetAllAsync()
        {
            try
            {
                var datas = await _unitOfWork.Repository<PaymentModel>()
                    .Query()
                    .Include(p => p.Booking) 
                    .OrderByDescending(p => p.CreatedDate)
                    .ToListAsync();

                return ServiceResult<IEnumerable<PaymentModel>>.Success(datas);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<PaymentModel>>.Failed($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PaymentModel>> GetByIdAsync(Guid id)
        {
            try
            {
                var data = await _unitOfWork.Repository<PaymentModel>().GetByIdAsync(id);
                if (data == null)
                    return ServiceResult<PaymentModel>.Failed("Không tìm thấy thông tin thanh toán.");

                return ServiceResult<PaymentModel>.Success(data);
            }
            catch (Exception ex)
            {
                return ServiceResult<PaymentModel>.Failed($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PaymentModel>> GetByBookingIdAsync(int bookingId)
        {
            try
            {
                var data = _unitOfWork.Repository<PaymentModel>()
                    .Query()
                    .Where(p => p.BookingId == bookingId)
                    .OrderByDescending(p => p.CreatedDate)
                    .FirstOrDefault();

                if (data == null)
                    return ServiceResult<PaymentModel>.Failed("Booking này chưa có dữ liệu thanh toán.");

                return ServiceResult<PaymentModel>.Success(data);
            }
            catch (Exception ex)
            {
                return ServiceResult<PaymentModel>.Failed($"Error: {ex.Message}");
            }
        }
    }
}
