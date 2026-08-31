using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Services;
using Osta.Infrastructure.Abstract.ServicesAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.ServiceImplemention
{
    public class ServiceRepository : GenericRepositoryAsync<Service>, IServiceRepository
    {
        #region Vars / Props
        private readonly DbSet<Service> Service;
        #endregion
        #region Constructor(s)
        public ServiceRepository(OstaContext dbContext) : base(dbContext)
        {
            Service = dbContext.Set<Service>();
        }

        public async Task<IEnumerable<Service>> GetAllWithCategoryAsync(CancellationToken cancellationToken)
        {
            return await Service
               .Include(s => s.Category)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> DoesCategoryHaveServiceAsync(int CategoryId, CancellationToken cancellationToken)
        {
            var Count = await Service.Include(a => a.Category).Where(a => a.CategoryId == CategoryId).CountAsync(cancellationToken);
            if (Count > 0) return true;
            return false;
        }
        #endregion

    }
}
