using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public class BookingDetail
    {
        public int Booking_Detail_Id { get; set; }

        [Required(ErrorMessage = "Booking không được để trống")]
        public int Booking_Id { get; set; }

        public int? Service_Id { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không vượt quá 500 ký tự")]
        public string? Description { get; set; }
        
        public int TotalDuration { get; set; }

        public DateTime Created_At { get; set; }
        public DateTime Update_At { get; set; }

        // Navigation
        public Booking Booking { get; set; }
        public Service Service { get; set; }
    }
}
