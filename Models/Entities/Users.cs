using BookingSalon.Data.Validation;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Models.Entities
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
        public string? Avatar_Name { get; set; }
        public string RoleId { get; set; }
        public bool Status { get; set; }
        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? Avatar_Image_Upload { get; set; }

        public StylistProfile? StylistProfile { get; set; }
        public ICollection<Booking>? CustormerBookings { get; set; }
        public ICollection<StylistTimeWork>? StylistTimeWorks { get; set; }
    }
}
