using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Booking;
using Osta.Infrastructure.Abstract.BookingAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.BookingImplemention
{
    public class BookingRepository : GenericRepositoryAsync<Bookings>, IBookingRepository
    {
        #region Vars / Props
        private readonly DbSet<Bookings> Bookings;
        #endregion
        #region Constructor(s)
        public BookingRepository(OstaContext dbContext) : base(dbContext)
        {
            Bookings = dbContext.Set<Bookings>();
        }

        #endregion

    }
}
