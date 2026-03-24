namespace BookingSalon.Models.Entities
{
    public class ComboServiceModel
    {
        public int ComboServiceId { get;set; }
        public int ComboId { get; set; }
        public int ServiceId { get; set; }
        public ServiceModel? Service { get; set; }
        public ComboModel? Combo { get; set; }
    }
}
