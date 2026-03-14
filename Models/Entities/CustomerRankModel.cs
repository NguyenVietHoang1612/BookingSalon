using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Models.Entities
{
    public class CustomerRankModel
    {
        [Key]
        [Display(Name = "Mã khách hàng")]
        public string Customer_Id { get; set; }

        [Required(ErrorMessage = "Điểm hiện tại không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Điểm hiện tại không được là số âm")]
        [Display(Name = "Điểm hiện tại")]
        public int CurrentPoints { get; set; }

        [Required(ErrorMessage = "Tổng điểm tích lũy không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Tổng điểm tích lũy không được là số âm")]
        [Display(Name = "Tổng điểm tích lũy")]
        public int LifetimePoints { get; set; }

        [Display(Name = "Hạng hiện tại")]
        public int? RankId { get; set; }

        [Display(Name = "Trạng thái")]
        public bool Is_Active { get; set; }

        public DateTime Update_At { get; set; }
        public DateTime Created_At { get; set; }

        // Navigation Properties
        public RankModel? Rank { get; set; }
        public UsersModel? Customer { get; set; }
    }
}