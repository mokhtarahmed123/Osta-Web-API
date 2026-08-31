using Osta.Data.Entities.Booking;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Abstract.BookingAbstract
{
    public interface IBookingRepository : IGenericRepositoryAsync<Bookings>
    {
    }
}
