using BookingSalon.Data.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Models.Entities
{

    public enum PortfolioCategory
    {
        Hair = 0,
        Skin = 1,
    }

    public class StaffPortfolioModel
    {
        [Key]
        public int ImageId { get; set; }

        public string? StaffProfileId { get; set; } 

        [Required(ErrorMessage = "Loại ảnh không được để trống")]
        public PortfolioCategory Category { get; set; }

        [StringLength(50, ErrorMessage = "Tiêu đề không vượt quá 50 ký tự")]
        public string? Title { get; set; }

        [StringLength(100, ErrorMessage = "Đường dẫn ảnh không vượt quá 255 ký tự")]
        public string? ImageUrl { get; set; }
        public bool IsFeatured { get; set; } 

        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }
        public StaffProfileModel? StaffProfile { get; set; }

        [NotMapped]
        [FileExtension(ErrorMessage = "Chỉ chấp nhận file ảnh (jpg, png, jpeg)")]
        [Required(ErrorMessage = "Ảnh minh họa không được để trống")]
        public IFormFile? Staff_Portfolio_Image_Upload { get; set; }
    }
}
