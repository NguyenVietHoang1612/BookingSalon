namespace BookingSalon.Areas.Admin.Models.ViewModel
{
    public class SlotSelectionViewModel
    {
        public int SlotId { get; set; }
        public TimeSpan? TimeRange { get; set; } 
        public bool IsAvailable { get; set; }  
    }
}
