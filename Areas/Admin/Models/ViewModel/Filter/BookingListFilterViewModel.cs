using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel.BookingSalonViewModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Areas.Admin.Models.ViewModel.Filter
{
    public class BookingListFilterViewModel
    {
        public PaginatedList<BookingProfileDetailsViewModel> Bookings { get; set; }

        public string? SearchTerm { get; set; }
        public BookingStatus? Status { get; set; }
        public int? BranchId { get; set; }

        public IEnumerable<BranchModel> Branches { get; set; }
    }
}
