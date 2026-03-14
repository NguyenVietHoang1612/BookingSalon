using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel.BookingSalonViewModel;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class StatisticalService : IStatisticalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingService _bookingService;
        private readonly IServicesSalonService _serviceSalon;
        private readonly IBranchService _branchService;
        private readonly IStaffProfileService _stylistProfileService;
        private readonly IUsersService _usersService;
        private readonly INewsService _newsService;
        private readonly IRankService _rankService;
        private readonly ICouponService _couponService;
        private readonly IStaffPortfolioService _staffPortfolioService;
        private readonly IFixedTimeSlotService _fixedTimeSlotService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public StatisticalService(IUnitOfWork unitOfWork, IBookingService bookingService, IServicesSalonService serviceSalon,
            IBranchService branchService, IStaffProfileService stylistProfileService, IUsersService usersService,
            INewsService newsService, IRankService rankService, ICouponService couponService, IStaffPortfolioService staffPortfolioService,
            IFixedTimeSlotService fixedTimeSlotService, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _serviceSalon = serviceSalon;
            _bookingService = bookingService;
            _branchService = branchService;
            _stylistProfileService = stylistProfileService;
            _usersService = usersService;
            _newsService = newsService;
            _rankService = rankService;
            _couponService = couponService;
            _staffPortfolioService = staffPortfolioService;
            _fixedTimeSlotService = fixedTimeSlotService;
            _roleManager = roleManager;
        }

        public async Task<int> GetNumberOfBranch()
        {
            var listBranch = await _branchService.GetAllAsync();
            int countbranch = listBranch.Data.Count();
            return countbranch;
        }

        public async Task<int> GetNumberOfStylist()
        {
            var listStylist = await _usersService.GetAllStylistAsync();
            int countStylist = listStylist.Count();
            return countStylist;
        }

        public async Task<int> GetNumberOfSkinner()
        {
            var listSkinner = await _usersService.GetAllSkinnerAsync();
            int countSkinner = listSkinner.Count();
            return countSkinner;
        }

        public async Task<int> GetNumberOfNews()
        {
            var listNew = await _newsService.GetAllAsync();
            var newActive = listNew.Where(s => s.IsActive == true);
            int countNew = newActive.Count();
            return countNew;
        }

        public async Task<int> GetNumberOfRank()
        {
            var listRank = await _rankService.GetAllAsync();
            int countRank = listRank.Data.Count();
            return countRank;
        }

        public async Task<int> GetNumberOfCoupon()
        {
            var listCoupon = await _couponService.GetAllAsync();
            int countCoupon = listCoupon.Data.Count();
            return countCoupon;
        }

        public async Task<int> GetNumberOfPorfolioAsync()
        {
            var listPortfolio = await _staffPortfolioService.GetAllAsync();
            var listActive = listPortfolio.Data.Where(s => s.IsFeatured == true);
            var listCount = listActive.Count();
            return listCount;
        }

        public async Task<int> GetNumberOfCustomerAsync()
        {
            var listCustomer = await _usersService.GetAllCustomerAsync();
            var listCount = listCustomer.Count();
            return listCount;
        }

        public async Task<int> GetNumberOfReceptionAsync()
        {
            var listReception = await _usersService.GetAllReceptionAsync();
            var listCount = listReception.Count();
            return listCount;
        }

        public async Task<decimal> TotalRevenueinMounth()
        {
            var booking = await _bookingService.GetAllAsync();
            var totalRevenue = booking
            .Where(b => b.Status == BookingStatus.Paid && b.Date_Booking.Month == DateTime.Now.Month)
            .Sum(b => (decimal?)b.FinalPrice) ?? 0;
            return totalRevenue;
        }

        public async Task<int> GetNumberOfServiceAsync()
        {
            var listService = await _serviceSalon.GetAllServiceActiveAsync();
            var listCount = listService.Count();
            return listCount;
        }

        public Task<IEnumerable<BookingProfileDetailsViewModel>> GetAllBookingAsync()
        {
            var listBooking = _bookingService.GetAllAsync();
            return listBooking;
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate)
        {
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var revenue = _unitOfWork.Repository<BookingModel>().Query()
                .Where(b => b.Booking_Date >= startDate && b.Status == BookingStatus.Completed)
                .Sum(b => b.TotalPrice);
            return revenue;
        }

        // Thong ke tong doanh thu moi chi nhanh
        public async Task<dynamic> GetRevenueData(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.Today.AddDays(-7);
            endDate ??= DateTime.Now;

            try
            {
                var allDates = Enumerable.Range(0, 1 + (endDate.Value - startDate.Value).Days)
                                         .Select(offset => startDate.Value.AddDays(offset).Date)
                                         .ToList();

                var rawData = _unitOfWork.Repository<BookingModel>().Query()
                    .Include(b => b.Branch)
                    .Where(b => b.Booking_Date >= startDate && b.Booking_Date <= endDate && b.Status == BookingStatus.Paid)
                    .GroupBy(b => new { b.Booking_Date.Date, b.Branch.Branch_Name })
                    .Select(g => new { Date = g.Key.Date, BranchName = g.Key.Branch_Name, Total = g.Sum(b => b.TotalPrice) })
                    .ToList();

                var branches = rawData.Select(x => x.BranchName).Distinct().ToList();

                var series = branches.Select(branch => new
                {
                    name = branch,
                    data = allDates.Select(d =>
                        rawData.FirstOrDefault(r => r.BranchName == branch && r.Date == d)?.Total ?? 0
                    ).ToList()
                });

                return new { dates = allDates.Select(d => d.ToString("yyyy-MM-dd")), series };
            }
            catch (Exception ex)
            {
                throw new Exception("Error...", ex);
            }
        }


        // Thống kê Doanh thu Nhân viên (Stylist/Skinner)
        public async Task<IEnumerable<StaffRevenueInfo>> GetStaffRevenueReportAsync(DateTime startDate, DateTime endDate, string role)
        {
            var query = _unitOfWork.Repository<BookingModel>().Query()
                .Where(b => b.Booking_Date >= startDate && b.Booking_Date <= endDate && b.Status == BookingStatus.Paid);

            if (role == "Stylist")
            {
                return await query
                    .Include(b => b.StylistProfile.Staff)
                    .Include(b => b.Branch)
                    .GroupBy(b => new
                    {
                        b.Stylist_Id,
                        b.StylistProfile.Staff.FullName,
                        b.StylistProfile.Staff.Email,
                        BranchName = b.Branch.Branch_Name
                    })
                    .Select(g => new StaffRevenueInfo
                    {
                        Date = startDate,
                        StaffId = g.Key.Stylist_Id,
                        StaffName = g.Key.FullName,
                        Email = g.Key.Email,
                        BranchName = g.Key.BranchName,
                        RoleName = role,
                        TotalBookings = g.Count(),
                        TotalRevenue = g.Sum(b => b.FinalPrice)
                    })
                    .OrderByDescending(x => x.TotalRevenue) 
                    .ToListAsync();
            }
            else if (role == "Skinner")
            {
                return await query
                    .Where(b => b.Skinner_Id != null)
                    .Include(b => b.SkinnerProfile.Staff)
                    .Include(b => b.Branch)
                    .GroupBy(b => new
                    {
                        b.Skinner_Id,
                        b.SkinnerProfile.Staff.FullName,
                        b.SkinnerProfile.Staff.Email,
                        BranchName = b.Branch.Branch_Name
                    })
                    .Select(g => new StaffRevenueInfo
                    {
                        Date = startDate,
                        StaffId = g.Key.Skinner_Id,
                        StaffName = g.Key.FullName,
                        Email = g.Key.Email,
                        BranchName = g.Key.BranchName,
                        RoleName = role,
                        TotalBookings = g.Count(),
                        TotalRevenue = g.Sum(b => b.FinalPrice)
                    })
                    .OrderByDescending(x => x.TotalRevenue)
                    .ToListAsync();
            }

            return new List<StaffRevenueInfo>();
        }

        //Thống kê Doanh thu Dịch vụ
        public async Task<IEnumerable<ServiceRevenueInfo>> GetServiceRevenueReportAsync(DateTime startDate, DateTime endDate)
        {
            var details = await _unitOfWork.Repository<BookingDetailModel>().Query()
                .Where(d => d.Booking.Booking_Date >= startDate &&
                            d.Booking.Booking_Date <= endDate &&
                            d.Booking.Status == BookingStatus.Paid)
                .ToListAsync();

            var totalRevenueAll = details.Sum(d => d.Price);

            return details
                .GroupBy(d => new { d.Service_Id, d.ServiceNameSnapshot })
                .Select(g => new ServiceRevenueInfo
                {
                    ServiceId = g.Key.Service_Id,
                    ServiceName = g.Key.ServiceNameSnapshot ?? "Dịch vụ đã xóa",
                    UsageCount = g.Count(),
                    TotalRevenue = g.Sum(x => x.Price),
                    Percentage = totalRevenueAll > 0 ? (double)(g.Sum(x => x.Price) / totalRevenueAll * 100) : 0
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();
        }
    }
}
