using BookingSalon.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Areas.Admin.Models.ViewModel
{
    public class UserUpdateViewModel
    {
        public UsersModel User { get; set; } = new UsersModel();
        public CustomerRankModel? CustomerRank { get; set; } = new CustomerRankModel();
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }

        public string? CustomerRole { get; set; }
    }
}
