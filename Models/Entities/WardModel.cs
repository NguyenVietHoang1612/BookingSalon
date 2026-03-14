namespace BookingSalon.Models.Entities
{
    public class WardModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public int DistrictId { get; set; }
        public DistrictModel? District { get; set; }
    }
}
