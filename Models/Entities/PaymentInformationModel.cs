namespace BookingSalon.Models.Entities
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public int BookingId { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
    }

}
