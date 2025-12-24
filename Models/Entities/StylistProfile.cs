using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public class StylistProfile
    {
        [Required(ErrorMessage = "Stylist không được để trống")]
        public string StylistId { get; set; }

        [Required(ErrorMessage = "Chi nhánh không được để trống")]
        public int Branch_Id { get; set; }

        [Required(ErrorMessage = "Kỹ năng stylist không được để trống")]
        [StringLength(255, ErrorMessage = "Kỹ năng không vượt quá 255 ký tự")]
        public string StylistSkill { get; set; }

        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm không hợp lệ")]
        public int StylistExperience { get; set; }

        [Required(ErrorMessage = "Giờ bắt đầu làm việc không được để trống")]
        public TimeSpan Start_Work_Time { get; set; }

        [Required(ErrorMessage = "Giờ kết thúc làm việc không được để trống")]
        public TimeSpan End_Work_Time { get; set; }

        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        // Navigation
        public Users Stylist { get; set; }
        public Branch Branch { get; set; }
        public ICollection<StylistImage>? StylistImages { get; set; }
    }
}

