namespace BookingSalon.Models.Entities
{
    public class ComboModel
    {
        public int ComboId { get; set; }
        public string ComboName { get; set; }

        public int TotalDuration { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PromotionPrice { get; set; }

        public ICollection<ComboServiceModel>? ComboServices { get; set; }
        public ICollection<BookingDetailModel>? BookingDetails { get; set; }
    }
}
