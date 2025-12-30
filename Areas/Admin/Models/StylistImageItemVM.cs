namespace BookingSalon.Areas.Admin.Models
{
    public class StylistImageItemVM
    {
        public int? ImageId { get; set; }        
        public string? ImageUrl { get; set; }    
        public bool IsRemoved { get; set; }
    }
}
