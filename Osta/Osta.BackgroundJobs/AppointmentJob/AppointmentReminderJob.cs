
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
=======
using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Data.Entities.Technician;
>>>>>>> c73a0b6 (Initial commit)
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
<<<<<<< HEAD
            var upcomingAppointments = await appointmentRepository.GetTableNoTracking(ct).Include
                    (a => a.Booking).ThenInclude(b => b.Customer)
            .Include(a => a.Booking).ThenInclude(b => b.Technician)
            .Where(a => a.ScheduledEnd >= now
                     && a.ScheduledStart <= windowEnd
                     && !a.ReminderSent
                     && a.IsApproved == true)
            .ToListAsync();

            return upcomingAppointments;
=======
            return await appointmentRepository
                .GetTableNoTracking(ct)
                .Where(a => a.ScheduledEnd >= now
                         && a.ScheduledStart <= windowEnd
                         && !a.ReminderSent
                         && a.IsApproved)
                .Select(a => new Appointment
                {
                    Id = a.Id,
                    BookingId = a.BookingId,
                    ScheduledStart = a.ScheduledStart,
                    ScheduledEnd = a.ScheduledEnd,
                    ReminderSent = a.ReminderSent,
                    IsApproved = a.IsApproved,
                    Booking = new Bookings
                    {
                        Id = a.Booking.Id,
                        BookingDate = a.Booking.BookingDate,
                        CustomerId = a.Booking.CustomerId,
                        TechnicianId = a.Booking.TechnicianId,
                        Customer = new User
                        {
                            Id = a.Booking.Customer.Id,
                            FullName = a.Booking.Customer.FullName,
                            Email = a.Booking.Customer.Email
                        },
                        Technician = new Technicians
                        {
                            Id = a.Booking.Technician.Id,
                        }
                    }
                })
                .ToListAsync(ct);
>>>>>>> c73a0b6 (Initial commit)
        }
    }
}
