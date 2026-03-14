using BookingSalon.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.ViewModel
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0|\+84)[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int? WardId { get; set; }
        public IFormFile? Avatar_Image_Upload { get; set; }
        public string? CurrentAvatar { get; set; }

        public string? RankName { get; set; }
        public int CurrentPoints { get; set; }
        public int LifetimePoints { get; set; }
        public decimal DiscountPercent { get; set; }
        public int? PointsToNextRank { get; set; }
        public string? NextRankName { get; set; }

        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
    }
}
