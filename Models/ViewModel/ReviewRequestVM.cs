namespace BookingSalon.Models.ViewModel
{
    public class ReviewRequestVM
    {
        public int BookingId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
