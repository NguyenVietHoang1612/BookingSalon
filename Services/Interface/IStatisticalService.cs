using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel.BookingSalonViewModel;

namespace BookingSalon.Services.Interface
{
    public interface IStatisticalService
    {
        Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate);
        Task<int> GetNumberOfBranch();
        Task<int> GetNumberOfCustomerAsync();
        Task<int> GetNumberOfStylist();
        Task<int> GetNumberOfSkinner();
        Task<int> GetNumberOfNews();
        Task<int> GetNumberOfRank();
        Task<int> GetNumberOfCoupon();
        Task<int> GetNumberOfPorfolioAsync();
        Task<decimal> TotalRevenueinMounth();
        Task<int> GetNumberOfReceptionAsync();
        Task<int> GetNumberOfServiceAsync();
        Task<IEnumerable<BookingProfileDetailsViewModel>> GetAllBookingAsync();
        Task<IEnumerable<ServiceRevenueInfo>> GetServiceRevenueReportAsync(DateTime startDate, DateTime endDate);

        Task<dynamic> GetRevenueData(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<StaffRevenueInfo>> GetStaffRevenueReportAsync(DateTime startDate, DateTime endDate, string role);
    }
}
