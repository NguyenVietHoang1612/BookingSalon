using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public class TypeOfService
    {
        public int TypeOfServiceId { get; set; }

        [Required(ErrorMessage = "Loại dịch vụ không được để trống")]
        [StringLength(100, ErrorMessage = "Loại dịch vụ không vượt quá 100 ký tự")]
        public string Type_Service_Name { get; set; }

        public DateTime Created_At { get; set; }
        public DateTime Update_At { get; set; }

        public ICollection<Service>? Services { get; set; }
    }
}
