using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Payment___Reviews;
using Osta.Infrastructure.Abstract.PaymentAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.PaymentImplementation
{
    public class UsageCouponsRepository : GenericRepositoryAsync<CouponUsage>, IUsageCouponsRepository
    {
        #region Vars / Props
        private readonly DbSet<CouponUsage> CouponUsage;
        #endregion
        #region Constructor(s)
        public UsageCouponsRepository(OstaContext dbContext) : base(dbContext)
        {
            CouponUsage = dbContext.Set<CouponUsage>();
        }
        #endregion

    }
}
