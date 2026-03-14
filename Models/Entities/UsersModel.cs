using BookingSalon.Data.Validation;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Models.Entities
{
    public class UsersModel : IdentityUser
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(50, ErrorMessage = "Họ tên không vượt quá 150 ký tự")]
        public string FullName { get; set; }

        [StringLength(100)]
        public string? Avatar_Name { get; set; }

        [Required(ErrorMessage = "Vai trò người dùng không được để trống")]
        public string RoleId { get; set; }

        [StringLength(150, ErrorMessage = "Địa chỉ không vượt quá 255 ký tự")]
        public string? Address { get; set; }

        public int? WardId { get; set; }
        public DateOnly? DateOfBirth { get; set; }

        public bool Status { get; set; }

        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }
        public WardModel? Ward { get; set; }

        [NotMapped]
        [FileExtension(ErrorMessage = "Chỉ chấp nhận file ảnh (jpg, png, jpeg)")]
        public IFormFile? Avatar_Image_Upload { get; set; }
    }
}
