using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;

namespace BookingSalon.Models.ViewModel
{
    public class NewsPageVM
    {
        public PaginatedList<NewsModel> NewsList { get; set; }
        public List<NewsModel> FeaturedNews { get; set; }
    }
}
