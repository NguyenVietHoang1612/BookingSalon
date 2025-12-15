namespace BookingSalon.Models.Entities
{
    public class Combo
    {
        public int Combo_Id { get; set; }
        public string Name_Combo { get; set; }
        public string Description { get; set; }
        public int Duration_Minutes { get; set; }
        public decimal ComboPrice { get; set; }
        public bool Status { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Update_At { get; set; }

        public ICollection<BookingDetail> BookingDetails { get; set; }
        public ICollection<ComboService> ComboServices { get; set; }
    }
}
