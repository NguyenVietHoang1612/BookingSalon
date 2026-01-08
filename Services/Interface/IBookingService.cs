using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel.BookingSalonViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookingSalon.Services.Interface
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingProfileDetailsViewModel>> GetAllAsync();
        Task<IEnumerable<Branch>> GetAllBranchesAsync();
        Task<IEnumerable<FixedTimeSlot>> GetAllFixedTimeSlotsAsync();
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<IEnumerable<TypeOfService>> GetAllTypeOfServiceAsync();
        Task<IEnumerable<StylistProfile>> GetAllStylistAsync();
        Task<ServiceResult<BookingDetailViewModel>> GetAllBookingDetailsAsync(int id);
        Task<ServiceResult<IEnumerable<StylistTimeWork>>> GetSlots(string stylistId, string dateStr);


        Task<ServiceResult<Booking>> GetByIdAsync(int id);
        Task<ServiceResult<BookingViewModel>> CreateAsync(BookingViewModel bookingVM);
        Task<ServiceResult<Booking>> UpdateAsync(int id, string status);
        Task<ServiceResult<Booking>> DeleteAsync(int id);

        Task<PaginatedList<BookingProfileDetailsViewModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm);
    }
}
