using Microsoft.AspNetCore.Identity;

namespace BookingSalon.Models.Entities
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
        public string? Avatar_Url { get; set; }
        public bool Status { get; set; }
        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        public StylistProfile? StylistProfile { get; set; }
        public ICollection<Booking> CustormerBookings { get; set; }
        public ICollection<StylistTimeWork> StylistTimeWorks { get; set; }
    }
}
