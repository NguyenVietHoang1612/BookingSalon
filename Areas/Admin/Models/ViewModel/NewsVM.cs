using BookingSalon.Models.Entities;

namespace BookingSalon.Areas.Admin.Models.ViewModel
{
    public class NewsVM
    {
        public NewsModel News { get; set; } = new NewsModel();
        public IFormFile? ImageUpload { get; set; }
    }
}
