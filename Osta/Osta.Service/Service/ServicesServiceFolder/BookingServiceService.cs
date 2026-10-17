using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities;
using Osta.Infrastructure.Abstract.ServicesAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Service.Service.ServicesServiceFolder
{
    public class BookingServiceService : IBookingServiceService
    {
        private readonly IBookingServicesRepository bookingServicesRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILoggerService loggerService;

        public BookingServiceService(IBookingServicesRepository bookingServicesRepository, IUnitOfWork unitOfWork, ILoggerService loggerService)
        {
            this.bookingServicesRepository = bookingServicesRepository;
            this.unitOfWork = unitOfWork;
            this.loggerService = loggerService;
        }
        public async Task Add(BookingService bookingService, CancellationToken cancellationToken)
        {
            await bookingServicesRepository.AddAsync(bookingService, cancellationToken);
            await unitOfWork.SaveChangesAsync();


        }

        public async Task Delete(int BookingId, int ServiceId, CancellationToken ct = default)
        {
            using var transaction = unitOfWork.BeginTransactionAsync();
            try
            {
                var BookingService = await bookingServicesRepository.
                    FirstOrDefaultAsync(x => x.ServiceId == ServiceId && x.BookingId == BookingId, ct);
                await bookingServicesRepository.DeleteAsync(BookingService, ct);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
            }
        }

        public async Task DeleteRangeByBookingId(
     int bookingId, CancellationToken ct = default)
        {
            await using var transaction =
                await unitOfWork.BeginTransactionAsync();

            try
            {
                var services =
                    await bookingServicesRepository
                        .GetTableAsTracking(ct)
                        .Where(x => x.BookingId == bookingId)
                        .ToListAsync();

                if (!services.Any())
                    return;

                await bookingServicesRepository.DeleteRangeAsync(services, ct);

                await unitOfWork.SaveChangesAsync();

                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<BookingService>> GetAll(CancellationToken cancellationToken)
        {
            return await bookingServicesRepository.GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<BookingService>> GetByBookingId(int BookingId, CancellationToken ct = default)
        {
            return await bookingServicesRepository.GetTableAsTracking(ct).Where(a => a.BookingId == BookingId).ToListAsync();
        }

        public async Task<IEnumerable<BookingService>> GetByServiceId(int ServiceId, CancellationToken ct = default)
        {
            return await bookingServicesRepository.GetTableAsTracking(ct).Where(a => a.ServiceId == ServiceId).ToListAsync();
        }
        public async Task Update(
            int bookingId,
            int serviceId,
            BookingService bookingService,
            CancellationToken cancellationToken)
        {
            if (bookingService is null)
                throw new ArgumentNullException(nameof(bookingService));

            var existingBookingService =
                await bookingServicesRepository.FirstOrDefaultAsync(
                    x => x.BookingId == bookingId &&

                         x.ServiceId == serviceId, cancellationToken);

            if (existingBookingService is null)
                throw new KeyNotFoundException(
                    "Booking service not found.");

            existingBookingService.PriceAtBooking = bookingService.PriceAtBooking;

            await bookingServicesRepository.UpdateAsync(
                existingBookingService, cancellationToken);

            await unitOfWork.SaveChangesAsync(
                    );
        }
    }
}
