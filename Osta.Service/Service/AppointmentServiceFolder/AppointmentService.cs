using Microsoft.EntityFrameworkCore;
using Osta.Core.HandlerMiddleware;
using Osta.Domain.Entities.Appointment;
using Osta.Infrastructure.Abstract.AppointmentAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.AppointmentAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Service.Service.AppointmentServiceFolder
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILoggerService loggerService;

        public AppointmentService(IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork, ILoggerService loggerService)
        {
            this.appointmentRepository = appointmentRepository;
            this.unitOfWork = unitOfWork;
            this.loggerService = loggerService;
        }
        public async Task AddAppointmentAsync(Appointment appointment, CancellationToken cancellationToken = default)
        {
            try
            {
                await appointmentRepository.AddAsync(appointment, cancellationToken);
                await unitOfWork.SaveChangesAsync();
                loggerService.LogInformation("Appointment added successfully for Booking ID: {BookingId}", appointment.BookingId);
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"An error occurred while adding Appointment for Booking ID: {appointment?.BookingId}");
                throw;
            }
        }

        public async Task ApproveAppointmentAsync(string id, CancellationToken ct = default)
        {
            var appointment = await appointmentRepository.GetByIdAsync(id, ct);
            if (appointment == null) return;
            appointment.IsApproved = true;
            await appointmentRepository.UpdateAsync(appointment, ct); await unitOfWork.SaveChangesAsync();
        }

        public async Task RejectAppointmentAsync(string id, CancellationToken ct = default)
        {
            var appointment = await appointmentRepository.GetByIdAsync(id, ct);
            if (appointment == null) return;
            appointment.IsApproved = false;
            await appointmentRepository.UpdateAsync(appointment, ct); await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAppointmentAsync(
       string id,
       CancellationToken cancellationToken = default)
        {
            try
            {

                var appointment =
                    await appointmentRepository.GetByIdAsync(id, cancellationToken);

                if (appointment is null)
                    throw new NotFoundException(
                        $"Appointment with ID {id} was not found.");

                await appointmentRepository.DeleteAsync(appointment, cancellationToken);

                await unitOfWork.SaveChangesAsync();

                loggerService.LogInformation(
                    "Appointment with ID {AppointmentId} deleted successfully.",
                    id);
            }
            catch (Exception ex)
            {
                loggerService.LogError(
                    ex,
                    "An error occurred while deleting Appointment with ID {AppointmentId}.",
                    id);

                throw;
            }
        }

        public async Task<Appointment?> Get(string id, CancellationToken ct = default)
        {
            return await appointmentRepository.GetByIdAsync(id, ct);
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(CancellationToken ct = default)
        {
            try
            {
                return await appointmentRepository
                    .GetTableNoTracking(ct)
                    .ToListAsync(ct);
            }
            catch (Exception ex)
            {
                loggerService.LogError(
                    ex,
                    "An error occurred while getting all appointments.");

                throw;
            }
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsByUserIdAsync(string userid, CancellationToken ct = default)
        {
            return await appointmentRepository.GetTableAsTracking(ct).
                Where(a => a.Booking.CustomerId == userid || a.Booking.TechnicianId == userid).ToListAsync();
        }

        public IQueryable<Appointment> GetAppointmentQueryable(CancellationToken ct = default)
        {
            try
            {
                return appointmentRepository
                    .GetTableNoTracking(ct).AsQueryable();
            }
            catch (Exception ex)
            {
                loggerService.LogError(
                    ex,
                    "An error occurred while getting appointments queryable.");

                throw;
            }
        }

        public async Task<Appointment?> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            return await appointmentRepository.FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken);
        }

        public async Task<bool> HasConflictAsync(string id, string technicianId, DateTime scheduledStart, DateTime scheduledEnd, CancellationToken ct = default)
        {

            return await appointmentRepository
                   .GetTableNoTracking(ct)
                   .AnyAsync(x =>
                   x.Id != id &&
                       x.Booking.TechnicianId == technicianId &&
                       scheduledStart < x.ScheduledEnd &&
                       scheduledEnd > x.ScheduledStart,
                       ct);
        }

        public async Task UpdateAppointmentAsync(
            string id,
            Appointment appointment,
            CancellationToken ct = default)
        {
            try
            {

                var existingAppointment =
                    await appointmentRepository.GetByIdAsync(id, ct);

                if (existingAppointment is null)
                    return;
                existingAppointment.ScheduledStart = appointment.ScheduledStart;
                existingAppointment.ScheduledEnd = appointment.ScheduledEnd;
                existingAppointment.IsApproved = appointment.IsApproved;
                existingAppointment.Notes = appointment.Notes;
                await appointmentRepository.UpdateAsync(existingAppointment, ct);

                await unitOfWork.SaveChangesAsync();

                loggerService.LogInformation(
                    "Appointment with ID {AppointmentId} updated successfully.",
                    id);
            }
            catch (Exception ex)
            {
                loggerService.LogError(
                    ex,
                    "An error occurred while updating Appointment with ID {AppointmentId}.",
                    id);

                throw;
            }
        }

        public async Task MakeReminderSentTrue(string AppointmentId, CancellationToken ct = default)
        {
            var appointment = await appointmentRepository.GetByIdAsync(AppointmentId, ct);
            if (appointment is null) return;
            appointment.ReminderSent = true;
            await appointmentRepository.UpdateAsync(appointment, ct);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Appointment>> CheckUpcomingAppointmentsAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var windowEnd = now.AddHours(1);
            var upcomingAppointments = await appointmentRepository.GetTableNoTracking(ct).Include
                    (a => a.Booking).ThenInclude(b => b.Customer)
            .Include(a => a.Booking).ThenInclude(b => b.Technician).ThenInclude(x => x.User)
            .Where(a => a.ScheduledEnd >= now
                     && a.ScheduledStart <= windowEnd
                     && !a.ReminderSent
                     && a.IsApproved == true)
            .ToListAsync();

            return upcomingAppointments;

        }
    }
}
