using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.TechnicianImplemention
{
    public class ServiceAreaRepository : GenericRepositoryAsync<ServiceArea>, IServiceAreaRepository
    {
        #region Vars / Props
        private readonly DbSet<ServiceArea> ServiceArea;
        #endregion
        #region Constructor(s)
        public ServiceAreaRepository(OstaContext dbContext) : base(dbContext)
        {
            ServiceArea = dbContext.Set<ServiceArea>();
        }

        #endregion

    }
}
