
using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Appointment;
using Osta.Infrastructure.Abstract.AppointmentAbstract;

namespace Osta.BackgroundJobs.AppointmentJob
{
    public class AppointmentReminderJob : IAppointmentReminderJob
    {

        private readonly IAppointmentRepository appointmentRepository;

        public AppointmentReminderJob(IAppointmentRepository appointmentRepository)
        {

            this.appointmentRepository = appointmentRepository;
        }
        public async Task<List<Appointment>> CheckUpcomingAppointmentsAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var windowEnd = now.AddHours(1);
            var upcomingAppointments = await appointmentRepository.GetTableNoTracking(ct).Include
                    (a => a.Booking).ThenInclude(b => b.Customer)
            .Include(a => a.Booking).ThenInclude(b => b.Technician)
            .Where(a => a.ScheduledEnd >= now
                     && a.ScheduledStart <= windowEnd
                     && !a.ReminderSent
                     && a.IsApproved == true)
            .ToListAsync();

            return upcomingAppointments;
        }
    }
}
