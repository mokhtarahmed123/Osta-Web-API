using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.TechnicianImplemention
{
    public class TechnicianAvailabilityRepository : GenericRepositoryAsync<TechnicianAvailability>, ITechnicianAvailabilityRepository
    {

        #region Vars / Props
        private readonly DbSet<TechnicianAvailability> TechnicianAvailability;
        #endregion
        #region Constructor(s)
        public TechnicianAvailabilityRepository(OstaContext dbContext) : base(dbContext)
        {
            TechnicianAvailability = dbContext.Set<TechnicianAvailability>();
        }

        #endregion
    }
}
