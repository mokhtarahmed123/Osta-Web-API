using Osta.Domain.Entities.Customer;
using Osta.Domain.Entities.Payment___Reviews;

namespace Osta.Payment.CouponService
{
    public interface ICouponService
    {
        Task AddAsync(Coupons coupon, CancellationToken ct);
        Task<Coupons?> GetByCodeAsync(string code, CancellationToken ct);
        Task<Coupons?> GetByIdAsync(int id, CancellationToken ct);
        Task AddRangeAsync(List<Coupons> coupons, CancellationToken ct);
<<<<<<< HEAD

        Task<List<Coupons>> GetAllAsync(bool isActive, CancellationToken ct);
        Task UpdateAsync(int Id, Coupons coupon, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);


=======
        Task<List<Coupons>> GetAllAsync(bool isActive, CancellationToken ct);
        Task UpdateAsync(int Id, Coupons coupon, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
>>>>>>> c73a0b6 (Initial commit)
        Task<bool> HasUserUsedCouponAsync(int couponId, string userId, CancellationToken ct);
        Task AddUsageAsync(CouponUsage usage, CancellationToken ct);
    }
}
