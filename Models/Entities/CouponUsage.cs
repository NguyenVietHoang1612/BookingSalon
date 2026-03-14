namespace BookingSalon.Models.Entities
{
    public class CouponUsage
    {
        public int Id { get; set; }

        public int CouponId { get; set; }
        public CouponModel Coupon { get; set; }

        public string UserId { get; set; }
        public UsersModel User { get; set; }

        public DateTime UsedAt { get; set; }
    }
}
