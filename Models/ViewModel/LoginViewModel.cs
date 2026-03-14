using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Nhớ")]
        public bool RememberMe { get; set; }
    }
}
