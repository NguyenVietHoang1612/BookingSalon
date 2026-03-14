using BookingSalon.Models.Entities;

namespace BookingSalon.Models.ViewModel.BookingSalonViewModel
{
    public class BookingDetailViewModel
    {
        public int BookingId { get; set; }
        public string Customer_Name { get; set; }
        public string Customer_Phone { get; set; }
        public string Customer_Email { get; set; }
        public string Customer_Address { get; set; }
        public string Id_Stylist { get; set; }
        public string Stylist_Name { get; set; }
        public string Id_Skinner { get; set; }
        public string Skinner_Name { get; set; }
        public string Branch_Name { get; set; }
        public string Branch_Address { get; set; }
        public string Branch_Phone { get; set; }
        public DateTime Date_Booking { get; set; }
        public TimeSpan TimeSlot { get; set; }
        public int TotalDuration { get; set; }
        public BookingStatus Status { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal? DiscoutValueSnapshot { get; set; }
        public DiscountType? DiscountTypeSnapshot { get; set; }
        public string? CouponCodeSnapshot { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime Create_At { get; set; }

        public List<string>? ResultImage { get; set; }
        public IEnumerable<BookingDetailModel>? bookingDetails { get; set; }
    }
}
