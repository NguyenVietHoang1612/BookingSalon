using BookingSalon.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Areas.Admin.Models.ViewModel
{
    public class StaffSelfUpdateVM
    {
        public UsersModel User { get; set; } = new UsersModel();

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string StaffId { get; set; } = default!;

        public int Branch_Id { get; set; }
        public string? Branch_name { get; set; }

        public TimeSpan Start_Work_Time { get; set; }

        public TimeSpan End_Work_Time { get; set; }

        [StringLength(250)]
        public string? Staff_Bio { get; set; }

        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
    }
}
