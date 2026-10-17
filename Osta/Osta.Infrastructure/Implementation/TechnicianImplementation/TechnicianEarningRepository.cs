using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.TechnicianImplementation
{
    public class TechnicianEarningRepository : GenericRepositoryAsync<TechnicianEarning>, ITechnicianEarningRepository
    {
        #region Vars / Props
        private readonly DbSet<TechnicianEarning> TechnicianEarning;
        #endregion
        #region Constructor(s)
        public TechnicianEarningRepository(OstaContext dbContext) : base(dbContext)
        {
            TechnicianEarning = dbContext.Set<TechnicianEarning>();
        }

        #endregion
    }
}
