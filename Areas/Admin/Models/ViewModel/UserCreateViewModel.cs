using BookingSalon.Models.Entities;
using DocumentFormat.OpenXml.Presentation;
using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Areas.Admin.Models.ViewModel
{
    public class UserCreateViewModel
    {
        [Required(ErrorMessage = "Không được để trống trường bắt buộc")]
        public UsersModel User { get; set; } = new UsersModel();
        public CustomerRankModel? CustomerRank { get; set; } = new CustomerRankModel();

        public string? CustomerRoleId { get; set; }
    }
}
