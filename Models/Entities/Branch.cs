namespace BookingSalon.Models.Entities
{
    public class Branch
    {
        public int BranchId { get; set; }
        public string Branch_Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool Status { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Update_At { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<StylistProfile>? StylistProfiles { get; set; }
    }
}
