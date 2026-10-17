namespace Osta.Core.Feature.Coupon.Command.Result
{
    public record ApplyCouponResult
    {
        public int CouponId { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountApplied { get; set; }
        public decimal FinalAmount { get; set; }
    }
}
