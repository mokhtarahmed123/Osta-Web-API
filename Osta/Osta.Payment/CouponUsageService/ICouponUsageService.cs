using Osta.Domain.Entities.Payment___Reviews;

namespace Osta.Payment.CouponUsageService
{
    public interface ICouponUsageService
    {
        Task AddAsync(CouponUsage couponUsage, CancellationToken cancellationToken);
    }
}
