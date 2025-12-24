using BookingSalon.Models.Entities;

namespace BookingSalon.Areas.Admin.Models
{
    public class BookingViewModel
    {
        public Booking Booking { get; set; }

        public List<BookingDetail> BookingDetails { get; set; }

        public List<Service> Services { get; set; }
    }
}
