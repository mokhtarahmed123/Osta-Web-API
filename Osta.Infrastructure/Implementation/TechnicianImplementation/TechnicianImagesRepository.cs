using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.TechnicianImplemention
{
    public class TechnicianImagesRepository : GenericRepositoryAsync<TechnicianImages>, ITechnicianImagesRepository
    {

        #region Vars / Props
        private readonly DbSet<TechnicianImages> TechnicianImages;
        #endregion
        #region Constructor(s)
        public TechnicianImagesRepository(OstaContext dbContext) : base(dbContext)
        {
            TechnicianImages = dbContext.Set<TechnicianImages>();
        }

        #endregion

    }
}
