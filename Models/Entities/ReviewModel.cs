using System;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public class ReviewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Booking_Id { get; set; }

        [Required]
        public string Customer_Id { get; set; }

        [Display(Name = "Người phản hồi")]
        public string? RepliedById { get; set; }

        [Display(Name = "Nhân viên được đánh giá")]
        public string? Staff_Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn số sao đánh giá")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        [Display(Name = "Số sao")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Nội dung đánh giá không được để trống")]
        [StringLength(500, ErrorMessage = "Nội dung đánh giá không vượt quá 500 ký tự")]
        [Display(Name = "Bình luận")]
        public string Comment { get; set; }

        [StringLength(500, ErrorMessage = "Nội dung phản hồi không vượt quá 500 ký tự")]
        [Display(Name = "Phản hồi từ Salon")]
        public string? Reply_Comment { get; set; }

        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        public BookingModel? Booking { get; set; }
        public UsersModel? Customer { get; set; } 
        public UsersModel? RepliedBy { get; set; }
        public UsersModel? Staff { get; set; }
    }
}