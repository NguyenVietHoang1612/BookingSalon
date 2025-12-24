using System;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public class Booking
    {
        public int Booking_Id { get; set; }

        [Required(ErrorMessage = "Khách hàng không được để trống")]
        public string Customer_Id { get; set; }

        [Required(ErrorMessage = "Chi nhánh không được để trống")]
        public int Branch_Id { get; set; }

        [Required(ErrorMessage = "Nhân viên tạo lịch không được để trống")]
        public string Stylist_Profile_Id { get; set; }

        [Required(ErrorMessage = "Khung giờ không được để trống")]
        public int Slot_Id { get; set; }

        [Required(ErrorMessage = "Ngày đặt lịch không được để trống")]
        [DataType(DataType.Date)]
        public DateTime Booking_Date { get; set; }

        [Required(ErrorMessage = "Trạng thái booking không được để trống")]
        [StringLength(50, ErrorMessage = "Trạng thái không vượt quá 50 ký tự")]
        public string Status { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền không hợp lệ")]
        public decimal TotalPrice { get; set; }

        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        public Users Customer { get; set; }
        public Branch Branch { get; set; }
        public StylistProfile StylistProfile { get; set; }
        public FixedTimeSlot StartSlot { get; set; }
        public Payment Payment { get; set; }
    }
}
