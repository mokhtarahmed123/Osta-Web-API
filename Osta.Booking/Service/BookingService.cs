using Microsoft.EntityFrameworkCore;
using Osta.Booking.Interface;
using Osta.Booking.Model;
using Osta.Data.Entities.Booking;
using Osta.Data.Enum;
using Osta.Infrastructure.Abstract.BookingAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.SharedKernel.Logging;

namespace Osta.Booking.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository bookingRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILoggerService loggerService;

        public BookingService(IBookingRepository bookingRepository, IUnitOfWork unitOfWork, ILoggerService loggerService)
        {
            this.bookingRepository = bookingRepository;
            this.unitOfWork = unitOfWork;
            this.loggerService = loggerService;
        }
        public async Task AddBooking(Bookings booking, CancellationToken ct = default)
        {
            try
            {
                await bookingRepository.AddAsync(booking, ct);
                await unitOfWork.SaveChangesAsync();
                loggerService.LogInformation("Booking added successfully to the database.");
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "An error occurred while adding booking.");
                throw;
            }
        }

        public async Task CancelBooking(int id, CancellationToken ct = default) // 
        {
            try
            {
                var booking = await bookingRepository.GetByIdAsync(id, ct);
                if (booking == null)
                    return;
                booking.Status = BookingStatus.Cancelled;
                await unitOfWork.SaveChangesAsync();
                loggerService.LogInformation("Booking with ID {BookingId} cancelled successfully.", id);
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "An error occurred while cancelling booking with ID {BookingId}.", id);
                throw;
            }
        }

        public async Task CompleteBooking(int id, CancellationToken ct = default)
        {
            try
            {
                var booking = await bookingRepository.GetByIdAsync(id, ct);
                if (booking == null)
                    return;
                booking.Status = BookingStatus.Completed;
                await unitOfWork.SaveChangesAsync();
                loggerService.LogInformation("Booking with ID {BookingId} Completed successfully.", id);
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "An error occurred while Completing booking with ID {BookingId}.", id);
                throw;
            }
        }

        public async Task ConfirmBooking(int id, CancellationToken ct = default)
        {
            try
            {
                var booking = await bookingRepository.GetByIdAsync(id, ct);
                if (booking == null)
                    return;
                booking.Status = Data.Enum.BookingStatus.Confirmed;
                await unitOfWork.SaveChangesAsync();
                loggerService.LogInformation("Booking with ID {BookingId} Confirmed successfully.", id);
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "An error occurred while Confirming booking with ID {BookingId}.", id);
                throw;
            }
        }

        public async Task DeleteBooking(int id, CancellationToken ct = default)
        {
            await using var transaction =
                await unitOfWork.BeginTransactionAsync();

            try
            {
                var booking = await bookingRepository.GetByIdAsync(id, ct);

                if (booking is null)
                {
                    loggerService.LogWarning(
                        "Booking with ID {BookingId} was not found.",
                        id);

                    return;
                }

                await bookingRepository.DeleteAsync(booking, ct);

                await unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                loggerService.LogInformation(
                    "Booking with ID {BookingId} deleted successfully.",
                    id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                loggerService.LogError(
                    ex,
                    "An error occurred while deleting booking with ID {BookingId}.",
                    id);

                throw;
            }
        }



        public async Task<Bookings?> GetBookingById(int id, CancellationToken ct = default)
        {
            return await bookingRepository.GetByIdAsync(id, ct);
        }

        public async Task<IEnumerable<Bookings>> GetBookingByStatus(BookingStatus status, CancellationToken ct = default)
        {
            return await bookingRepository.GetTableNoTracking(ct).Where(x => x.Status == status).ToListAsync();

        }

        public async Task<IEnumerable<GetAllBookingsAsCustomerdto>>
            GetBookingsByClientId(string clientId, CancellationToken ct = default)
        {
            return await bookingRepository
                .GetTableNoTracking(ct)
                .Where(x => x.CustomerId == clientId)
                .Select(x => new GetAllBookingsAsCustomerdto
                {
                    BookingId = x.Id,
                    TechnicianId = x.TechnicianId,
                    TechnicianName = x.Technician.User.FullName,
                    TechnicianEmail = x.Technician.User.Email,
                    CustomerName = x.Customer.FullName,
                    Status = x.Status.ToString(),
                    CustomerEmail = x.Customer.Email,
                    Area = x.Area,
                    City = x.City,
                    Governorate = x.Governorate,
                    Street = x.Street,
                    BookingDate = x.BookingDate
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<Bookings>> GetBookingsByTechnicianId(string technicianId, CancellationToken ct = default)
        {
            return await bookingRepository.GetTableNoTracking(ct).Include(x => x.Customer).
                Where(x => x.TechnicianId == technicianId).ToListAsync();
        }

        public async Task RefuseBooking(int id, CancellationToken ct = default)
        {
            try
            {
                var booking = await bookingRepository.GetByIdAsync(id, ct);
                if (booking == null)
                    return;
                booking.Status = BookingStatus.Refused;
                await unitOfWork.SaveChangesAsync();
                loggerService.LogInformation("Booking with ID {BookingId} Refused successfully.", id);
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "An error occurred while Refusing booking with ID {BookingId}.", id);
                throw;
            }

        }


        public async Task UpdateBooking(int id, Bookings booking, CancellationToken ct = default)
        {
            try
            {
                var book = await bookingRepository.GetByIdAsync(id, ct);
                if (book == null)
                    return;

                book.Area = booking.Area;
                book.City = booking.City;
                book.Governorate = booking.Governorate;
                book.Street = booking.Street;

                await unitOfWork.SaveChangesAsync();

                loggerService.LogInformation(
                    "Booking with ID {BookingId} updated successfully.",
                    id);
            }
            catch (Exception ex)
            {
                loggerService.LogError(
                    ex,
                    "An error occurred while updating booking with ID {BookingId}.",
                    id);

                throw;
            }
        }


    }
}
