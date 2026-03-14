using BookingSalon.Models.Entities;

namespace BookingSalon.Models.ViewModel
{
    public class HomeViewModel
    {
        public IEnumerable<StylistList> StylistLists { get; set; }
        public IEnumerable<SkinnertList> SkinnertList { get; set; }

        public IEnumerable<StylistImageHome> StylistImageList { get; set; }
        public IEnumerable<NewsModel> NewsList { get; set; }
    }

    public class StylistList
    {
        public string stylistName;
        public string nameImage;
        public string AddressBranch;
        public double rating;
    }

    public class SkinnertList
    {
        public string skinnerName;
        public string nameImage;
        public string AddressBranch;
    }


    public class StylistImageHome
    {
        public string title;
        public string nameStylist;
        public string nameImage;
    }
}
