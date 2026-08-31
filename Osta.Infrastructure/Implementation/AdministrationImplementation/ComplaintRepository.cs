using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Administration;
using Osta.Infrastructure.Abstract.AdministrationAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.AdministrationImplementation
{
    public class ComplaintRepository : GenericRepositoryAsync<Complaint>, IComplaintRepository
    {
        #region Vars / Props
        private readonly DbSet<Complaint> Complaint;
        #endregion
        #region Constructor(s)
        public ComplaintRepository(OstaContext dbContext) : base(dbContext)
        {
            Complaint = dbContext.Set<Complaint>();
        }

        #endregion

    }
}
