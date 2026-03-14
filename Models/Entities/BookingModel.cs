using System;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public enum BookingStatus : byte
    {
        [Display(Name = "Chờ xác nhận")]
        PendingConfirmation = 0,

        [Display(Name = "Đã xác nhận")]
        Confirmed = 1,

        [Display(Name = "Đang phục vụ")]
        InProgress = 2,

        [Display(Name = "Hoàn thành")]
        Completed = 3,

        [Display(Name = "Đã thanh toán")]
        Paid = 4,

        [Display(Name = "Đã hủy")]
        Canceled = 5
    }
    public class BookingModel
    {
        public int Booking_Id { get; set; }

        [Required(ErrorMessage = "Khách hàng không được để trống")]
        public string Customer_Id { get; set; }

        [Required(ErrorMessage = "Chi nhánh không được để trống")]
        public int Branch_Id { get; set; }

        [Required(ErrorMessage = "Stylist tạo lịch không được để trống")]
        public string Stylist_Id { get; set; }
        [Required(ErrorMessage = "Skinner tạo lịch không được để trống")]
        public string Skinner_Id { get; set; }
        [Required(ErrorMessage = "Khung giờ không được để trống")]
        public int Start_Slot_Id { get; set; }
        public int? End_Slot_Id { get; set; }
        [Required(ErrorMessage = "Ngày đặt lịch không được để trống")]
        [DataType(DataType.Date)]
        public DateTime Booking_Date { get; set; }

        public BookingStatus Status { get; set; }
        public int TotalDuration { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }  
        public decimal FinalPrice { get; set; }
        public int? Coupon_Id { get; set; }
        public string? CouponCodeSnapshot { get; set; }
        public decimal? DiscountValueSnapshot { get; set; }
        public DiscountType? DiscountTypeSnapshot { get; set; }
        [StringLength(250, ErrorMessage = "ghi chú không vượt quá 250 ký tự")]
        public string? Note { get; set; }
        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }
        public string? UpdatedById { get; set; }
        public UsersModel? UpdatedBy { get; set; }
        public UsersModel? Customer { get; set; }
        public BranchModel? Branch { get; set; }
        public StaffProfileModel? StylistProfile { get; set; }
        public StaffProfileModel? SkinnerProfile { get; set; }
        public FixedTimeSlotModel? StartSlot { get; set; }
        public FixedTimeSlotModel? EndSlot { get; set; }
        public CouponModel? Coupon { get; set; }
        public ICollection<BookingDetailModel>? BookingDetails { get; set; }
        public virtual ICollection<BookingImageModel>? BookingImages { get; set; }
    }
}
