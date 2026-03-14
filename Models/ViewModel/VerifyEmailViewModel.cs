using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.ViewModel
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress]
        public string Email { get; set; }

    }
}
