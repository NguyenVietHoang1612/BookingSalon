using BookingSalon.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Models.Entities
{
    public class PaymentModel
    {
        public Guid PaymentId { get; set; }

        public int BookingId { get; set; }
        
        public decimal Amount { get; set; } 
        public string PaymentMethod { get; set; } // "Tiền mặt", "VNPay"
        public string? OrderInfo { get; set; } 
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? PaymentDate { get; set; } 

        public string? ProcessedBy { get; set; }

        public UsersModel? StaffId { get; set; }
        public BookingModel? Booking { get; set; }
    }
}
