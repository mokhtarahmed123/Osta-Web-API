using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Services;
using Osta.Infrastructure.Abstract.ServicesAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.ServiceImplemention
{
    public class CategoryRepository : GenericRepositoryAsync<Category>, ICategoryRepository
    {
        #region Vars / Props
        private readonly DbSet<Category> Category;
        #endregion
        #region Constructor(s)
        public CategoryRepository(OstaContext dbContext) : base(dbContext)
        {
            Category = dbContext.Set<Category>();
        }

        #endregion
    }
}

