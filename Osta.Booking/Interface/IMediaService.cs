using Microsoft.AspNetCore.Http;
using Osta.Data.Entities.Booking;
using Osta.Domain.Enum;

namespace Osta.Booking.Interface
{
    public interface IMediaService
    {
        Task AddAsync(
            Media media, IFormFile file,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<Media>> GetByTypeAsync(int BookingId, RepairMediaTypeEnum type, CancellationToken cancellationToken = default);
        Task UpdateAsync(int Id, Media media, IFormFile file, CancellationToken cancellationToken = default);

        Task<IEnumerable<Media>> GetByBookingIdAsync(
            int bookingId,
            CancellationToken cancellationToken = default);

        Task<Media?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

    }
}
