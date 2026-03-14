using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public class BookingImageModel
    {
        [Key]
        public int Image_Id { get; set; }

        [Required]
        public int Booking_Id { get; set; }

        public string ImageUrl { get; set; }

        public DateTime Created_At { get; set; } = DateTime.Now;

        public virtual BookingModel? Booking { get; set; }
    }
}
