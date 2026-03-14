using BookingSalon.Data.Repository;
using BookingSalon.Migrations;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{

    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ReviewModel>> GetAllAsync()
        {
            return await _unitOfWork.Repository<ReviewModel>()
                .Query()
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .Include(r => r.Booking)
                .ToListAsync();
        }

        public async Task<ServiceResult<bool>> SubmitReviewAsync(int bookingId, int rating, string comment, string customerId)
        {
            try
            {
                var booking = await _unitOfWork.Repository<BookingModel>()
                    .Query()
                    .FirstOrDefaultAsync(b => b.Booking_Id == bookingId && b.Customer_Id == customerId);

                if (booking == null)
                    return ServiceResult<bool>.Failed("Không tìm thấy thông tin lịch hẹn.");

                if (booking.Status != BookingStatus.Paid)
                    return ServiceResult<bool>.Failed("Bạn chỉ có thể đánh giá sau khi đã hoàn thành dịch vụ.");

                bool alreadyReviewed = await _unitOfWork.Repository<ReviewModel>()
                    .Query()
                    .AnyAsync(r => r.Booking_Id == bookingId);

                if (alreadyReviewed)
                    return ServiceResult<bool>.Failed("Lịch hẹn này đã được đánh giá trước đó.");

                var review = new ReviewModel
                {
                    Booking_Id = bookingId,
                    Customer_Id = customerId,
                    Staff_Id = booking.Stylist_Id, 
                    Rating = rating,
                    Comment = comment,
                    Created_At = DateTime.Now
                };
                await _unitOfWork.Repository<ReviewModel>().AddAsync(review);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failed($"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<PaginatedList<ReviewModel>> GetPagedReviewsAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<ReviewModel>().Query();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.Comment.Contains(searchTerm) ||
                                         r.Customer.FullName.Contains(searchTerm) ||
                                         r.Staff.FullName.Contains(searchTerm));
            }

            query = query.Include(r => r.Customer)
                .Include(r => r.Staff)
                .Include(r => r.Booking)
                .OrderByDescending(r => r.Created_At);

            return await PaginatedList<ReviewModel>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedList<ReviewModel>> GetStaffPagedReviewsAsync(string staffId, int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<ReviewModel>().Query()
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .Include(r => r.Booking)
                .AsNoTracking()
                .Where(r => r.Staff_Id == staffId)
                .OrderByDescending(r => r.Created_At)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.Comment.Contains(searchTerm) ||
                                         r.Customer.FullName.Contains(searchTerm));
                                        
            }

            return await PaginatedList<ReviewModel>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedList<ReviewModel>> GetCustomerPagedReviewsAsync(string customerId, int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<ReviewModel>().Query()
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Branch)
                .OrderByDescending(r => r.Created_At)
                .AsQueryable();

            query = query.Where(r => r.Customer_Id == customerId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.Comment.Contains(searchTerm) ||
                                         r.Customer.FullName.Contains(searchTerm));

            }

            return await PaginatedList<ReviewModel>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<ServiceResult<ReviewModel>> ReplyReviewAsync(int reviewId, string replyComment, string adminId)
        {
            try
            {
                var review = await _unitOfWork.Repository<ReviewModel>().GetByIdAsync(reviewId);
                if (review == null) return ServiceResult<ReviewModel>.Failed("Không tìm thấy đánh giá.");

                review.Reply_Comment = replyComment;
                review.RepliedById = adminId;
                review.Updated_At = DateTime.Now;

                _unitOfWork.Repository<ReviewModel>().Update(review);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<ReviewModel>.Success(review);
            }
            catch (Exception ex)
            {
                return ServiceResult<ReviewModel>.Failed($"Lỗi khi phản hồi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteReviewAsync(int id)
        {
            try
            {
                var review = await _unitOfWork.Repository<ReviewModel>().GetByIdAsync(id);
                if (review == null) return ServiceResult<bool>.Failed("Không tìm thấy đánh giá");

                _unitOfWork.Repository<ReviewModel>().Delete(review);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failed(ex.Message);
            }
        }
    }
}
