using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities;
using Osta.Infrastructure.Abstract.ServicesAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.ServiceImplementation
{
    internal class BookingServicesRepository : GenericRepositoryAsync<BookingService>, IBookingServicesRepository
    {
        #region Vars / Props
        private readonly DbSet<BookingService> BookingService;
        #endregion
        #region Constructor(s)
        public BookingServicesRepository(OstaContext dbContext) : base(dbContext)
        {
            BookingService = dbContext.Set<BookingService>();
        }

        #endregion

    }
}
