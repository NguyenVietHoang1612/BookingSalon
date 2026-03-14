using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;

namespace BookingSalon.Services.Interface
{
    public interface IReviewService
    {

        Task<IEnumerable<ReviewModel>> GetAllAsync();
        Task<ServiceResult<bool>> SubmitReviewAsync(int bookingId, int rating, string comment, string customerId);

        Task<PaginatedList<ReviewModel>> GetPagedReviewsAsync(int pageNumber, int pageSize, string searchTerm);
        Task<PaginatedList<ReviewModel>> GetStaffPagedReviewsAsync(string staffId, int pageNumber, int pageSize, string searchTerm);
        Task<PaginatedList<ReviewModel>> GetCustomerPagedReviewsAsync(string customerId, int pageNumber, int pageSize, string searchTerm);
        Task<ServiceResult<ReviewModel>> ReplyReviewAsync(int reviewId, string replyComment, string adminId);
        Task<ServiceResult<bool>> DeleteReviewAsync(int id);
    }
}
