using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.TechnicianImplemention
{
    public class TechnicianServiceAreasRepository : GenericRepositoryAsync<TechnicianServiceArea>, ITechnicianServiceAreasRepository
    {
        #region Vars / Props
        private readonly DbSet<TechnicianServiceArea> TechnicianServiceArea;
        #endregion
        #region Constructor(s)
        public TechnicianServiceAreasRepository(OstaContext dbContext) : base(dbContext)
        {
            TechnicianServiceArea = dbContext.Set<TechnicianServiceArea>();
        }

        public async Task<bool> TechnicianHasThisServiceArea(string TechnicianId, int ServiceAreaId)
        {
            return await TechnicianServiceArea
                .AnyAsync(x => x.TechnicianId == TechnicianId &&
                               x.ServiceAreaId == ServiceAreaId);
        }
    }

    #endregion


}

