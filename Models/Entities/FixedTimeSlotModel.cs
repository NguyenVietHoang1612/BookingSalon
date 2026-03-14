using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public class FixedTimeSlotModel
    {
        public int SlotId { get; set; }

        [Required(ErrorMessage = "Thời gian không được để trống")]
        public TimeSpan TimeLabel { get; set; }

        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }
    }
}
