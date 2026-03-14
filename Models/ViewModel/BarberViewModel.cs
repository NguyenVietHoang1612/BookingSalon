using BookingSalon.Models.Entities;

namespace BookingSalon.Models.ViewModel
{
    public class BarberViewModel
    {

        public IEnumerable<BranchModel>? Branches { get; set; } = new List<BranchModel>();

        public IEnumerable<TypeOfServiceModel>? Categories { get; set; } = new List<TypeOfServiceModel>();

        public IEnumerable<StaffProfileModel>? FeaturedStylists { get; set; } = new List<StaffProfileModel>();
    }
}
