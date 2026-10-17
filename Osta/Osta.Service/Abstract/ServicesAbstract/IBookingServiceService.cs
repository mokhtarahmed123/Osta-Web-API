using Osta.Data.Entities;

namespace Osta.Service.Abstract.ServicesAbstract
{
    public interface IBookingServiceService
    {
        Task Add(BookingService bookingService, CancellationToken cancellationToken);
        Task Update(int BookingId, int ServiceId, BookingService bookingService, CancellationToken cancellationToken);
        Task Delete(int BookingId, int ServiceId, CancellationToken ct = default);
        Task<IEnumerable<BookingService>> GetByBookingId(int BookingId, CancellationToken ct = default);
        Task<IEnumerable<BookingService>> GetByServiceId(int ServiceId, CancellationToken ct = default);

        Task<IEnumerable<BookingService>> GetAll(CancellationToken cancellationToken);

        Task DeleteRangeByBookingId(int BookingId, CancellationToken ct = default);




    }
}
