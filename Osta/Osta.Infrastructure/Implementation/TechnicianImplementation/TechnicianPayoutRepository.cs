using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.TechnicianImplementation
{
    public class TechnicianPayoutRepository : GenericRepositoryAsync<TechnicianPayout>, ITechnicianPayoutRepository
    {
        #region Vars / Props
        private readonly DbSet<TechnicianPayout> TechnicianPayout;
        #endregion
        #region Constructor(s)
        public TechnicianPayoutRepository(OstaContext dbContext) : base(dbContext)
        {
            TechnicianPayout = dbContext.Set<TechnicianPayout>();
        }

        #endregion
    }
}
