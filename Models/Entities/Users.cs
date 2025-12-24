using BookingSalon.Data.Validation;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Models.Entities
{
    public class Users : IdentityUser
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(150, ErrorMessage = "Họ tên không vượt quá 150 ký tự")]
        public string FullName { get; set; }

        [StringLength(255)]
        public string? Avatar_Name { get; set; }

        [Required(ErrorMessage = "Vai trò người dùng không được để trống")]
        public string RoleId { get; set; }

        public bool Status { get; set; }

        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        [NotMapped]
        [FileExtension(ErrorMessage = "Chỉ chấp nhận file ảnh (jpg, png, jpeg)")]
        public IFormFile? Avatar_Image_Upload { get; set; }
    }
}
