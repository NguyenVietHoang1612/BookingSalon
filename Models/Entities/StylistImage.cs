using BookingSalon.Data.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Models.Entities
{
    public class StylistImage
    {
        public int ImageId { get; set; }

        [Required(ErrorMessage = "Stylist không được để trống")]
        public string StylistProfileId { get; set; }

        [Required(ErrorMessage = "Đường dẫn ảnh không được để trống")]
        [StringLength(255, ErrorMessage = "Đường dẫn ảnh không vượt quá 255 ký tự")]
        public string ImageUrl { get; set; }
        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        public StylistProfile StylistProfile { get; set; }
    }
}
