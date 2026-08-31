using Osta.Booking.Model;
using Osta.Data.Entities.Booking;
using Osta.Data.Enum;

namespace Osta.Booking.Interface
{
    public interface IBookingService
    {
        Task AddBooking(Bookings booking, CancellationToken ct = default);


        Task<Bookings?> GetBookingById(int id, CancellationToken ct = default);
        Task<IEnumerable<GetAllBookingsAsCustomerdto>> GetBookingsByClientId(string clientId, CancellationToken ct = default);
        Task<IEnumerable<Bookings>> GetBookingsByTechnicianId(string technicianId, CancellationToken ct = default);
        Task<IEnumerable<Bookings>> GetBookingByStatus(BookingStatus status, CancellationToken ct = default);


        Task UpdateBooking(int Id, Bookings booking, CancellationToken ct = default);


        Task DeleteBooking(int id, CancellationToken ct = default);


        Task CancelBooking(int id, CancellationToken ct = default);
        Task RefuseBooking(int id, CancellationToken ct = default);
        Task ConfirmBooking(int id, CancellationToken ct = default);
        Task CompleteBooking(int id, CancellationToken ct = default);


    }
}
