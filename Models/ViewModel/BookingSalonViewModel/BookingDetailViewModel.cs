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
        public string Branch_Name { get; set; }
        public string Branch_Address { get; set; }
        public DateOnly Date_Booking { get; set; }
        public TimeSpan TimeSlot { get; set; }
        public int TotalDuration { get; set; }
        public string Status { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime Create_At { get; set; }

        public IEnumerable<BookingDetail>? bookingDetails { get; set; }
    }
}
