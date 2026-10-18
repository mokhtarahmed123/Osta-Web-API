using Osta.Domain.Entities.Payment___Reviews;
using Osta.Infrastructure.Abstract.PaymentAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Payment.CouponUsageService;

namespace Osta.Service.PaymentService
{
    public class CouponUsageService : ICouponUsageService
    {
        private readonly IUsageCouponsRepository usageCouponsRepository;
        private readonly IUnitOfWork unitOfWork;

        public CouponUsageService(IUsageCouponsRepository usageCouponsRepository, IUnitOfWork unitOfWork)
        {
            this.usageCouponsRepository = usageCouponsRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task AddAsync(CouponUsage couponUsage, CancellationToken cancellationToken)
        {
            await usageCouponsRepository.AddAsync(couponUsage, cancellationToken);
            await unitOfWork.SaveChangesAsync();

        }
    }
}
