using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Customer;
using Osta.Domain.Entities.Payment___Reviews;
using Osta.Infrastructure.Abstract.PaymentAbstract;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Payment.CouponService
{
    public class CouponService : ICouponService
    {
        private readonly ICouponsRepository couponsRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUsageCouponsRepository usageCouponsRepository;

        public CouponService(ICouponsRepository couponsRepository, IUnitOfWork unitOfWork, IUsageCouponsRepository usageCouponsRepository)
        {
            this.couponsRepository = couponsRepository;
            this.unitOfWork = unitOfWork;
            this.usageCouponsRepository = usageCouponsRepository;
        }
        public async Task AddAsync(Coupons coupon, CancellationToken ct)
        {
            await couponsRepository.AddAsync(coupon, ct);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<Coupons> coupons, CancellationToken ct)
        {
            await couponsRepository.AddRangeAsync(coupons, ct);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task AddUsageAsync(CouponUsage usage, CancellationToken ct)
        {
            await usageCouponsRepository.AddAsync(usage, ct);
            await unitOfWork.SaveChangesAsync();

        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var coupon = await couponsRepository.GetByIdAsync(id, ct);

                if (coupon == null)
                    return false;
                await couponsRepository.DeleteAsync(coupon, ct);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<List<Coupons>> GetAllAsync(bool isActive, CancellationToken ct)
        {
            return await couponsRepository.GetTableNoTracking(ct).Where(c => c.IsActive == isActive).ToListAsync();
        }

        public async Task<Coupons?> GetByCodeAsync(string code, CancellationToken ct)
        {
            return await couponsRepository.FirstOrDefaultAsync(c => c.Code == code, ct);
        }

        public async Task<Coupons?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await couponsRepository.GetByIdAsync(id, ct);
        }

        public async Task<bool> HasUserUsedCouponAsync(int couponId, string userId, CancellationToken ct)
        {
            return await usageCouponsRepository.GetTableNoTracking(ct)
                   .AnyAsync(cu => cu.CouponId == couponId && cu.UserId == userId, ct);
        }

        public async Task UpdateAsync(int id, Coupons coupon, CancellationToken ct)
        {
            var old = await couponsRepository.GetByIdAsync(id, ct);

            if (old is null)
                throw new KeyNotFoundException($"Coupon with ID {id} not found.");

            old.DiscountType = coupon.DiscountType;
            old.DiscountValue = coupon.DiscountValue;
            old.StartDate = coupon.StartDate;
            old.EndDate = coupon.EndDate;
            old.UsageLimit = coupon.UsageLimit;
            old.IsActive = coupon.IsActive;

            await couponsRepository.UpdateAsync(old, ct);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
