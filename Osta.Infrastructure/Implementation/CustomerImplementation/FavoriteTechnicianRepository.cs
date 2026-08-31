using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities;
using Osta.Infrastructure.Abstract.CustomerAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.CustomerImplementation
{
    public class FavoriteTechnicianRepository : GenericRepositoryAsync<FavoriteTechnician>, IFavoriteTechnicianRepository
    {
        #region Vars / Props
        private readonly DbSet<FavoriteTechnician> FavoriteTechnician;
        #endregion
        #region Constructor(s)
        public FavoriteTechnicianRepository(OstaContext dbContext) : base(dbContext)
        {
            FavoriteTechnician = dbContext.Set<FavoriteTechnician>();
        }

        #endregion


    }
}
