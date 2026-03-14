using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public class RankModel
    {
        [Key]
        public int RankId { get; set; }

        [Required(ErrorMessage = "Tên hạng thành viên không được để trống")]
        [StringLength(50, ErrorMessage = "Tên hạng không được vượt quá 50 ký tự")]
        public string RankName { get; set; }

        [Required(ErrorMessage = "Điểm tối thiểu không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Điểm tối thiểu phải là số dương")]
        public int MinPoint { get; set; }

        [Required(ErrorMessage = "Số ngày đặt trước tối đa không được để trống")]
        [Range(1, 365, ErrorMessage = "Số ngày đặt trước phải từ 1 đến 365 ngày")]
        public int MaxBookingDays { get; set; }

        [Required(ErrorMessage = "Phần trăm giảm giá không được để trống")]
        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 100%")]
        public decimal DiscountPercent { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        public DateTime? Created_At { get; set; }
        public DateTime? Update_At { get; set; }
    }
}
