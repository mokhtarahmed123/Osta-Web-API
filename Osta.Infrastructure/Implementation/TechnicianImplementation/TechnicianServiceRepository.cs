using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.TechnicianImplemention
{
    public class TechnicianServiceRepository : GenericRepositoryAsync<TechnicianService>, ITechnicianServiceRepository
    {
        #region Vars / Props
        private readonly DbSet<TechnicianService> TechnicianService;
        #endregion
        #region Constructor(s)
        public TechnicianServiceRepository(OstaContext dbContext) : base(dbContext)
        {
            TechnicianService = dbContext.Set<TechnicianService>();
        }



        public async Task<IEnumerable<TechnicianService>> GetAllByServiceId(int serviceId)
        {
            return await TechnicianService.Where(x => x.ServiceId == serviceId).ToListAsync();
        }

        public async Task<IEnumerable<TechnicianService>> GetAllByTechnicianId(string TechnicianId)
        {
            return await TechnicianService.Where(x => x.TechnicianId == TechnicianId).ToListAsync();
        }





        #endregion

    }
}
