namespace BookingSalon.Models.Entities
{
    public class FixedTimeSlot
    {
        public int SlotId { get; set; }
        public TimeSpan TimeLabel { get; set; }
        public int Sort_Order { get; set; }
        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }
        public ICollection<StylistTimeWork> StylistTimeWorks { get; set; }
        public ICollection<Booking> Bookings { get; set; }
       
    }
}
