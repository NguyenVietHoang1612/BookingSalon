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

        public string? RoleId { get; set; }

        [StringLength(150, ErrorMessage = "Địa chỉ không vượt quá 255 ký tự")]
        public string? Address { get; set; }
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
        public override string? Email { get; set; }
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải đúng 10 chữ số")]
        [RegularExpression(@"^(0|\+84)[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public override string? PhoneNumber { get; set; }

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
