using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Customer;
using Osta.Infrastructure.Abstract.PaymentAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.PaymentImplementation
{
    public class CouponsRepository : GenericRepositoryAsync<Coupons>, ICouponsRepository
    {
        #region Vars / Props
        private readonly DbSet<Coupons> Coupons;
        #endregion
        #region Constructor(s)
        public CouponsRepository(OstaContext dbContext) : base(dbContext)
        {
            Coupons = dbContext.Set<Coupons>();
        }

        #endregion
    }
}
