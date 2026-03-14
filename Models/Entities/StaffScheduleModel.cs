using BookingSalon.Data.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Models.Entities
{
    public class StaffScheduleModel
    {
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "Staff không được để trống")]
        public string Staff_Id { get; set; }

        [Required(ErrorMessage = "Ngày làm việc không được để trống")]
        [DataType(DataType.Date)]
        public DateOnly Work_Date { get; set; }

        [Required(ErrorMessage = "Khung giờ không được để trống")]
        public int Slot_Id { get; set; }
        public int Booking_Id { get; set; }

        public DateTime? Create_At { get; set; }
        public DateTime? Update_At { get; set; }

        // Navigation
        public BookingModel? Booking { get; set; }
        public StaffProfileModel? StaffProfile { get; set; }
        public FixedTimeSlotModel? TimeSlot { get; set; }
    }

}

