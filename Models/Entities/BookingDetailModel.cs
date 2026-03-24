using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public class BookingDetailModel
    {
        public int Booking_Detail_Id { get; set; }

        [Required(ErrorMessage = "Booking không được để trống")]
        public int Booking_Id { get; set; }

        public int? Service_Id { get; set; }

        public int? Combo_Id { get; set; }

        [Required]
        public decimal Price { get; set; } 

        public decimal BasePriceSnapshot { get; set; } 

        public string? ServiceNameSnapshot { get; set; } 

        public int DurationSnapshot { get; set; } 

        public DateTime Created_At { get; set; }
        public DateTime Update_At { get; set; }

        public BookingModel? Booking { get; set; }
        public ServiceModel? Service { get; set; }

        public ComboModel? Combo { get; set; }
    }
}
