namespace BookingSalon.Models.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public DateTime Payment_Date { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public string TypePay { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Update_At { get; set; }
        public Booking Booking { get; set; }
    }
}
