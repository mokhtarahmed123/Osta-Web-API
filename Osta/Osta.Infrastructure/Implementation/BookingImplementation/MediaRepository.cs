using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Booking;
using Osta.Infrastructure.Abstract.BookingAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.BookingImplementation
{
    public class MediaRepository : GenericRepositoryAsync<Media>, IMediaRepository
    {
        #region Vars / Props
        private readonly DbSet<Media> Media;
        #endregion
        #region Constructor(s)
        public MediaRepository(OstaContext dbContext) : base(dbContext)
        {
            Media = dbContext.Set<Media>();
        }

        #endregion

    }
}
