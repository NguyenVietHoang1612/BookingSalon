using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.ViewModel
{
    public class AuthOtpViewModel
    {
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải đúng 10 chữ số")]
        [RegularExpression(@"^(0|84)[3|5|7|8|9][0-9]{8}$", ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Mã xác thực OTP")]
        [Required(ErrorMessage = "Mã OTP không được để trống")]
        public string? OtpCode { get; set; }
    }
}
