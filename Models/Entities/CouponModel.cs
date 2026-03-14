using System;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public enum DiscountType : byte
    {
        [Display(Name = "Phần trăm (%)")]
        Percentage = 0,
        [Display(Name = "Số tiền cố định (VND)")]
        FixedAmount = 1
    }

    public class CouponModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã giảm giá không được để trống")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Mã phải từ 3 đến 15 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Mã giảm giá chỉ được chứa chữ và số, không có khoảng trắng")]
        [Display(Name = "Mã Coupon")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Loại giảm giá là bắt buộc")]
        public DiscountType Discount_Type { get; set; }

        [Required(ErrorMessage = "Giá trị giảm không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn 0")]
        public decimal Discount_value { get; set; }

        [Required(ErrorMessage = "Giá trị đơn hàng tối thiểu không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị đơn hàng tối thiểu không được âm")]
        public decimal Min_Order_Amount { get; set; }

        [Required(ErrorMessage = "Giới hạn sử dụng không được để trống")]
        public int Usage_Limit { get; set; }

        public int Used_Count { get; set; }

        public int? MaxUsagePerUser { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn không được để trống")]
        [DataType(DataType.DateTime)]
        public DateTime Expires_At { get; set; }

        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
}