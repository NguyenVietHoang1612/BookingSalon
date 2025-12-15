namespace BookingSalon.Models.Entities
{
    public class StylistImage
    {
        public int ImageId { get; set; }
        public string StylistId { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }
        public StylistProfile StylistProfile { get; set; }
    }
}
