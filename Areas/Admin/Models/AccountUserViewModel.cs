using BookingSalon.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Areas.Admin.Models
{
    public class AccountUserViewModel
    {
        public Users User { get; set; } = new Users();

        [Required(ErrorMessage = "Password không được để trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
