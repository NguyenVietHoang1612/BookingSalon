using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Tên không được để trống.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "Mật khẩu phải có tối thiểu 4 ký tự và nhỏ hơn 40 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("ConfirmPassword", ErrorMessage = "mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
