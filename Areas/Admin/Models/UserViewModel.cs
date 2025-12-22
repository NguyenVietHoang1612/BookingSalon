using BookingSalon.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Areas.Admin.Models
{
    public class UserViewModel
    {
        
        public Users User { get; set; }
        public string RoleName { get; set; }
    }
}
