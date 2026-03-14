namespace BookingSalon.Models.Entities
{
    public class ProvinceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public ICollection<DistrictModel>? Districts { get; set; }
    }
}
