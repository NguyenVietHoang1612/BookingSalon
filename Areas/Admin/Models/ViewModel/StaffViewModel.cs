using BookingSalon.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Areas.Admin.Models.ViewModel
{
    public class StaffCombinedCreateVM
    {
        public UsersModel User { get; set; } = new UsersModel();

        [Required(ErrorMessage = "Password không được để trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public StaffProfileModel ProfileInfo { get; set; } = new StaffProfileModel();
        public SelectList? BranchSelectList { get; set; }
        public SelectList? RoleSelectList { get; set; }
    }

    public class StaffCombinedUpdateVM
    {
        public UsersModel User { get; set; } = new UsersModel();
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public StaffProfileModel ProfileInfo { get; set; } = new StaffProfileModel();
        public SelectList? BranchSelectList { get; set; }
        public SelectList? RoleSelectList { get; set; }
    }
}
