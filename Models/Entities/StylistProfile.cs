namespace BookingSalon.Models.Entities
{
    public class StylistProfile
    {
        public string StylistId { get; set; }
        public int Branch_Id { get; set; }
        public string StylistSkill { get; set; }   
        public int StylistExperience { get; set; }
        public TimeSpan Start_Work_Time { get; set; }
        public TimeSpan End_Work_Time { get; set; }
        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        public Users Stylist { get; set; }
        public Branch Branch { get; set; }
        public ICollection<StylistImage> StylistImages { get; set; }
    }
}

