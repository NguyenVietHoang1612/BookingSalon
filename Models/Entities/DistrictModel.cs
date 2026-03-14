namespace BookingSalon.Models.Entities
{
    public class DistrictModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int ProvinceId { get; set; }
        public ProvinceModel? Province { get; set; }
        public ICollection<WardModel>? Wards { get; set; }
    }
}
