namespace BookingSalon.Models.Entities
{
    public class Booking
    {
        public int Booking_Id { get; set; }
        public string Customer_Id { get; set; }
        public int Branch_Id { get; set; }
        public string Stylist_Profile_Id { get; set; }
        public int Slot_Id { get; set; }
        public DateTime Booking_Date { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        public Users Customer { get; set; }
        public Branch Branch { get; set; }
        public StylistProfile StylistProfile{ get; set; }
        public FixedTimeSlot StartSlot { get; set; }
        public Payment Payment { get; set; }
        public ICollection<BookingDetail> BookingServices { get; set; }
    }
}
