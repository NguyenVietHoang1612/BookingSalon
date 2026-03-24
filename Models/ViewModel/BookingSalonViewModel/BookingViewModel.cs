using BookingSalon.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.ViewModel.BookingSalonViewModel
{
    public class BookingViewModel
    {
        public BookingModel NewBooking { get; set; }
        public List<BookingDetailModel>? BookingDetails { get; set; }

        public IEnumerable<BranchModel>? Branches { get; set; }
        public IEnumerable<TypeOfServiceModel>? TypeOfServices { get; set; }
        public IEnumerable<FixedTimeSlotModel>? TimeSlots { get; set; }
        public IEnumerable<ServiceModel>? Services { get; set; }
        public IEnumerable<StaffProfileModel>? Stylists { get; set; }
        public IEnumerable<StaffProfileModel>? Skinners { get; set; }
        public IEnumerable<UsersModel>? Customers { get; set; }
        public IEnumerable<ComboModel>? Combos { get; set; }
        public decimal RankPercent { get; set; }
        public string? CouponCode { get; set; }
    }
}
