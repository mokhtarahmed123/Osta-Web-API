using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Technician;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.TechnicianImplementation
{
    public class TechnicianWalletRepository : GenericRepositoryAsync<TechnicianWallet>, ITechnicianWalletRepository
    {
        #region Vars / Props
        private readonly DbSet<TechnicianWallet> TechnicianWallet;
        #endregion
        #region Constructor(s)
        public TechnicianWalletRepository(OstaContext dbContext) : base(dbContext)
        {
            TechnicianWallet = dbContext.Set<TechnicianWallet>();
        }

        #endregion
    }
}
