using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel;
using BookingSalon.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class HomeService : IHomeService
    {
        private readonly IUsersService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INewsService _newsService;
        private readonly IAddressService _addressService;
        private readonly IStaffProfileService _staffProfileService;
        private readonly IReviewService _reviewService;

        public HomeService(IUsersService userService, IUnitOfWork unitOfWork, INewsService newsService, IAddressService addressService, 
            IStaffProfileService staffProfileService, IReviewService reviewService)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _newsService = newsService;
            _addressService = addressService;
            _staffProfileService = staffProfileService;
            _reviewService = reviewService;
        }

        public async Task<HomeViewModel> GetAllStaffAsync()
        {
            var stylistList = await _staffProfileService.GetAllStylistActiveAsync();
            var allNews = await _newsService.GetAllAsync();
            var skinnerList = await _staffProfileService.GetAllSkinnerActiveAsync();
            var allReviews = await _reviewService.GetAllAsync();
            var stylists = new List<StylistList>();
            foreach (var s in stylistList)
            {
                var ward = await _addressService.GetAddressByWardIdAsync(s.Branch.WardId ?? 0);

                stylists.Add(new StylistList
                {
                    stylistName = s.Staff.FullName,
                    nameImage = s.Staff.Avatar_Name,
                    AddressBranch = ward != null
                        ? $"{s.Branch.Address}, {ward.Name}, {ward.District?.Name}, {ward.District?.Province?.Name}"
                        : "Chưa cập nhật",
                    rating = allReviews.Where(r => r.Staff_Id == s.StaffId).Any() 
                    ? Math.Round(allReviews.Where(r => r.Staff_Id == s.StaffId).Average(r => r.Rating), 1) 
                    : 5.0 
                });
            }

            var skinners = new List<SkinnertList>();
            foreach (var s in skinnerList)
            {
                var ward = await _addressService.GetAddressByWardIdAsync(s.Branch.WardId ?? 0);

                skinners.Add(new SkinnertList
                {
                    skinnerName = s.Staff.FullName,
                    nameImage = s.Staff.Avatar_Name,
                    AddressBranch = ward != null
                        ? $"{s.Branch.Address}, {ward.Name}, {ward.District?.Name}, {ward.District?.Province?.Name}"
                        : "Chưa cập nhật"
                });
            }


            var imageStylist = _unitOfWork.Repository<StaffPortfolioModel>()
                .Query()
                .Include(s => s.StaffProfile).ThenInclude(s => s.Staff)
                .Where(p => p.IsFeatured == true);

            var images = imageStylist.Select(i => new StylistImageHome
            {
                title = i.Title,
                nameImage = i.ImageUrl,
                nameStylist = i.StaffProfile.Staff.FullName
            }).ToList();

            return new HomeViewModel
            {
                StylistLists = stylists,
                StylistImageList = images,
                SkinnertList = skinners,
                NewsList = allNews.Where(x => x.IsActive).Take(3).ToList()
            };
        }
    }
}
