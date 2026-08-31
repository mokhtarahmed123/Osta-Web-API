using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.TechnicianImplemention
{
    public class TechnicianRepository : GenericRepositoryAsync<Technicians>, ITechnicianRepository
    {
        #region Vars / Props
        private readonly DbSet<Technicians> Technicians;
        #endregion
        #region Constructor(s)
        public TechnicianRepository(OstaContext dbContext) : base(dbContext)
        {
            Technicians = dbContext.Set<Technicians>();
        }

        public async Task<IEnumerable<Technicians>> GetAllTechniciansWithServiceArea(int ServiceAreaId)
        {
            return await Technicians
                .Where(t => t.TechnicianServiceArea.Any(sa => sa.ServiceAreaId == ServiceAreaId))
                .Include(t => t.TechnicianServiceArea)
                .ToListAsync();
        }

        public async Task<IEnumerable<Technicians>> GetAllWithServiceId(int serviceId)
        {
            return await Technicians
                .Where(t => t.TechnicianServices.Any(ts => ts.ServiceId == serviceId))
                .Include(t => t.TechnicianServices)
                .ToListAsync();
        }

        public async Task<Technicians> GetByIdWithServiceAndServiceArea(string Id)
        {
            return await Technicians
    .Include(t => t.TechnicianServices)
        .ThenInclude(ts => ts.Service)
    .Include(t => t.TechnicianServiceArea)
        .ThenInclude(tsa => tsa.ServiceArea).FirstOrDefaultAsync(x => x.Id == Id);
        }

        #endregion
    }
}
