namespace BookingSalon.Models.Entities
{
    public class BookingDetail
    {
        public int Booking_Detail_Id { get; set; }
        public int Booking_Id { get; set; }
        public int? Service_Id { get; set; }
        public int? ComboId { get; set; }
        public string Description { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Update_At { get; set; }

        public Booking Booking { get; set; }
        public Service Service { get; set; }
        public Combo Combo { get; set; }
    }
}
