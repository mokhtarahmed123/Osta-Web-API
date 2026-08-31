using Osta.Domain.Enum;

namespace Osta.Core.Feature.Coupon.Query.Result
{
    public record GetCouponByCodeResult
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public DiscountTypeEnum DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public bool IsActive { get; set; }
    }
}
