using BookingSalon.Models.Entities;

namespace BookingSalon.Areas.Admin.Models.ViewModel
{
    public class StatisticalViewModel
    {
        public List<StaffRevenueInfo> StylistRevenues { get; set; } = new();
        public List<StaffRevenueInfo> SkinnerRevenues { get; set; } = new();
        public List<ServiceRevenueInfo> ServiceRevenues { get; set; } = new(); 

        public List<BookingModel> NewBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCompletedBookings { get; set; }
    }

    public class StaffRevenueInfo
    {
        public DateTime Date { get; set; }
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public string BranchName { get; set; }
        public string RoleName { get; set; } 
        public string Email { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; } 
        public decimal AvgRevenuePerBooking => TotalBookings > 0 ? TotalRevenue / TotalBookings : 0;
    }

    public class ServiceRevenueInfo
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int UsageCount { get; set; } 
        public decimal TotalRevenue { get; set; } 
        public double Percentage { get; set; } 
    }

    public class BookingRatioViewModel
    {
        public int Total { get; set; }
        public int CompletedCount { get; set; }
        public int CanceledCount { get; set; }
        public double CancelRate { get; set; }
    }

    public class RevenueDataPoint
    {
       

    }
}

