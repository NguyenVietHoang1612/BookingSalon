using BookingSalon.Models.Entities;
using System.Globalization;

namespace BookingSalon.Areas.Admin.Models.ViewModel
{
    public class CustomerRankVM
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string? Address { get; set; }
        public string? AvatarName { get; set; }
        public string RankName { get; set; }
        public int? CurrentPoints { get; set; }
        public int? LifetimePoints { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool Status { get; set; }
        public bool EmailConfirm { get; set; }
        public bool ActiveRank { get; set; }

        public WardModel Ward { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
