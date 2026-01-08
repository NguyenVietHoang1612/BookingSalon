using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public class StylistTimeWork
    {
        public int Stylist_Time_Work_Id { get; set; }

        [Required(ErrorMessage = "Stylist không được để trống")]
        public string Stylist_Id { get; set; }

        [Required(ErrorMessage = "Ngày làm việc không được để trống")]
        [DataType(DataType.Date)]
        public DateOnly Work_Date { get; set; }

        [Required(ErrorMessage = "Khung giờ không được để trống")]
        public int Slot_Id { get; set; }

        public bool Is_Available { get; set; }

        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        // Navigation
        public Users Stylist { get; set; }
        public FixedTimeSlot TimeSlot { get; set; }
    }

}

