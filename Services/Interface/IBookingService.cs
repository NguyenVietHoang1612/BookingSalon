using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel.BookingSalonViewModel;


namespace BookingSalon.Services.Interface
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingProfileDetailsViewModel>> GetAllAsync();
        Task<ServiceResult<BookingDetailViewModel>> GetAllBookingDetailsAsync(int id);
        Task<ServiceResult<IEnumerable<SlotSelectionViewModel>>> GetSlots(string stylistId, string skinnerId, DateOnly date, int durationMinutes);
        Task<ServiceResult<BookingModel>> GetByIdAsync(int id);
        Task<ServiceResult<BookingViewModel>> BookingAsync(BookingViewModel bookingVM);
        Task<ServiceResult<BookingModel>> UpdateStatusAsync(int id, byte status, string updateById);
        Task<ServiceResult<BookingModel>> UpdateImageAsync(int id, List<IFormFile> imageFiles, string updateById);
        Task<PaginatedList<BookingProfileDetailsViewModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm, BookingStatus? status, int? branchId);
        Task<PaginatedList<BookingProfileDetailsViewModel>> GetPagedListBranchAsync(int branchID, int pageNumber, int pageSize, string searchTerm, BookingStatus? status);
        Task<PaginatedList<BookingProfileDetailsViewModel>> GetPagedListStaffIdAsync(string StaffId, int pageNumber, int pageSize, string searchTerm, BookingStatus? status);
        Task<ServiceResult<string?>> GetAvailableStylistIdAsync(int branchId, DateTime date, int startSlotId, int durationMinutes);
        Task<ServiceResult<string?>> GetAvailableSkinnerIdAsync(int branchId, DateTime date, int startSlotId, int durationMinutes, string? excludeStaffId = null);
        Task<ServiceResult<IEnumerable<SlotSelectionViewModel>>> GetSlotsAvailableForBranchRandom(int branchId, string? stylistId, string? skinnerId, DateOnly date, int durationMinutes);
    }
}
