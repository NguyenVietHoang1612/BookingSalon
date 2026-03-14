using BookingSalon.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Areas.Admin.Models.ViewModel
{
    public class UserViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? WardId { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public string? Address { get; set; }
        public string? avatarName { get; set; }

        public IFormFile? Avatar_Image_Upload { get; set; }
        public string? Password { get; set; }
    }
}
