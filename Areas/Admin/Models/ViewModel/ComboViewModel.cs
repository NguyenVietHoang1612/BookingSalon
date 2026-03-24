using BookingSalon.Models.Entities;

namespace BookingSalon.Areas.Admin.Models.ViewModel
{
    public class ComboViewModel
    {
        public List<ComboServiceModel> ComboServiceModel { get; set; }
        public ComboModel ComboModel { get; set; }

        public IEnumerable<ServiceModel> Services { get; set; }
    }
}
