using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public class FixedTimeSlot
    {
        public int SlotId { get; set; }

        [Required(ErrorMessage = "Thời gian không được để trống")]
        public TimeSpan TimeLabel { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự sắp xếp phải lớn hơn 0")]
        public int Sort_Order { get; set; }

        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }
    }
}
