using BookingSalon.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.ViewModel.BookingSalonViewModel
{
    public class BookingViewModel
    {
        //Customer Booking Create
        public Booking NewBooking { get; set; }
        public IEnumerable<BookingDetail>? BookingDetails { get; set; }

        public IEnumerable<Branch>? Branches { get; set; }
        public IEnumerable<TypeOfService>? TypeOfServices { get; set; }
        public IEnumerable<FixedTimeSlot>? TimeSlots { get; set; }
        public IEnumerable<Service>? Services { get; set; }
        public IEnumerable<Users>? Stylists { get; set; }

        public decimal? TotalPrice { get; set; }

    }
}
