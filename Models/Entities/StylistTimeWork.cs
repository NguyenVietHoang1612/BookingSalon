namespace BookingSalon.Models.Entities
{
    public class StylistTimeWork
    {
        public int Stylist_Time_Work_Id { get; set; }
        public string Stylist_Id { get; set; }
        public DateTime Work_Date { get; set; }
        public int Slot_Id { get; set; }
        public bool Is_Available { get; set; }
        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        public Users Stylist { get; set; }
        public FixedTimeSlot TimeSlot { get; set; }
    }

}

