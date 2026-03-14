using BookingSalon.Models.Entities;

namespace BookingSalon.Models.ViewModel
{
    public class CouponValidateViewModel
    {
        public decimal Discount { get; set; }
        public int CouponId { get; set; }
        public string Code { get; set; }
        public decimal DiscountValue { get; set; }
        public DiscountType DiscountType { get; set; }
    }
}
