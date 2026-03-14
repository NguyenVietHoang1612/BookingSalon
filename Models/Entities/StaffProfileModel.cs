using System.ComponentModel.DataAnnotations;

namespace BookingSalon.Models.Entities
{
    public class StaffProfileModel
    {
        [Required(ErrorMessage = "Staff Id không được để trống")]
        public string StaffId { get; set; }
        [Required(ErrorMessage = "Chi nhánh không được để trống")]
        public int Branch_Id { get; set; }
        [StringLength(250, ErrorMessage = "Kỹ năng không vượt quá 250 ký tự")]
        public string? Staff_Bio { get; set; }
        [Required(ErrorMessage = "Giờ bắt đầu làm việc không được để trống")]
        public TimeSpan Start_Work_Time { get; set; }
        [Required(ErrorMessage = "Giờ kết thúc làm việc không được để trống")]
        public TimeSpan End_Work_Time { get; set; }
        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        public UsersModel? Staff { get; set; }
        public BranchModel? Branch { get; set; }
        public ICollection<StaffPortfolioModel>? StaffPortfolio { get; set; }
    }
}

