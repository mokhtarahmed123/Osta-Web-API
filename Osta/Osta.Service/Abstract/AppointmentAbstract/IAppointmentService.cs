using Osta.Domain.Entities.Appointment;

namespace Osta.Service.Abstract.AppointmentAbstract
{
    public interface IAppointmentService
    {
        Task AddAppointmentAsync(Appointment appointment, CancellationToken ct = default);
        Task UpdateAppointmentAsync(string id, Appointment appointment, CancellationToken ct = default);

        Task<Appointment?> Get(string id, CancellationToken ct = default);
        Task DeleteAppointmentAsync(string id, CancellationToken ct = default);
        IQueryable<Appointment> GetAppointmentQueryable(CancellationToken ct = default);

        Task ApproveAppointmentAsync(string id, CancellationToken ct = default);
        Task RejectAppointmentAsync(string id, CancellationToken ct = default);
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(CancellationToken ct = default);
        Task<IEnumerable<Appointment>> GetAllAppointmentsByUserIdAsync(string userid, CancellationToken ct = default);
        Task<Appointment?> GetByBookingIdAsync(
int bookingId,
CancellationToken cancellationToken = default);
        Task<bool> HasConflictAsync(
            string? id,
   string technicianId,
   DateTime scheduledStart,
   DateTime scheduledEnd,
   CancellationToken ct = default);

        Task MakeReminderSentTrue(string AppointmentId, CancellationToken ct = default);
        Task<List<Appointment>> CheckUpcomingAppointmentsAsync(CancellationToken ct = default);
    }
}
