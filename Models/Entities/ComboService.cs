namespace BookingSalon.Models.Entities
{
    public class ComboService
    {
        public int ComboId { get; set; }
        public int ServiceId { get; set; }
        public string Description { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Update_At { get; set; }
        public Combo Combo { get; set; }
        public Service Service { get; set; }
    }
}
