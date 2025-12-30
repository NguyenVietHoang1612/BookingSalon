using BookingSalon.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Areas.Admin.Models
{
    public class UserUpdateViewModel
    {
        public Users User { get; set; } = new Users();

        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
