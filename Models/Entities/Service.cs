namespace BookingSalon.Models.Entities
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string Service_Name { get; set; }
        public decimal Price { get; set; }
        public string Type_Service { get; set; }
        public bool Status { get; set; }
        public int DurationInMinutes { get; set; }

        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        public ICollection<BookingDetail> BookingDetail { get; set; }
        public ICollection<ComboService> ComboServices { get; set; }
    }
}
