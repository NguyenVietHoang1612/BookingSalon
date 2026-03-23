using BookingSalon.Models.Entities;

namespace BookingSalon.Models.ViewModel.BookingSalonViewModel
{
    public class BookingProfileDetailsViewModel
    {
        public int BookingId { get; set; }
        public string Customer_Id { get; set; }
        public string Customer_Name { get; set; }
        public string Customer_Phone { get; set; }
        public string Customer_Email { get; set; }
        public string Customer_Address { get; set; }
        public string Id_Stylist { get; set; }
        public string Stylist_Name { get; set; }
        public string? Stylist_Image { get; set; }
        public string Id_Skinner { get; set; }
        public string Skinner_Name { get; set; }
        public string? Skinner_Image { get; set; }
        public string Branch_Name { get; set; }
        public string? UpdateById { get; set; }
        public string? UpdateByName { get; set; }
        public DateTime Date_Booking { get; set; }
        public TimeSpan TimeSlot { get; set; }
        public int? TotalDuration { get; set; } 
        public BookingStatus Status { get; set; }
        public decimal? FinalPrice { get; set; }
        public DateTime CreateAt { get; set; }
        public IEnumerable<BookingImageModel>? BookingImages { get; set; }

        public bool IsReview { get; set; }
    }
}
