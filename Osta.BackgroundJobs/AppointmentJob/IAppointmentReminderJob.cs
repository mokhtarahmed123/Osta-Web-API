using Osta.Domain.Entities.Appointment;

namespace Osta.BackgroundJobs.AppointmentJob
{
    public interface IAppointmentReminderJob
    {
        Task<List<Appointment>> CheckUpcomingAppointmentsAsync(CancellationToken ct = default);

    }
}
