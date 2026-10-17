using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Appointment.Command.Model.AppointmentJobMdoel;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.AppointmentAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Appointment.Command.Handler
{
    public class SendAppointmentRemindersCommandHandler : ResponseHandler, IRequestHandler<SendAppointmentRemindersCommand, Response<int>>
    {
        private readonly ISendNotificationMessage notificationService;

        private readonly ILoggerService loggerService;
        private readonly IAppointmentService appointmentService;


        public SendAppointmentRemindersCommandHandler(ISendNotificationMessage notificationService,
             ILoggerService loggerService, IAppointmentService appointmentService)
        {
            this.notificationService = notificationService;

            this.loggerService = loggerService;
            this.appointmentService = appointmentService;

        }
        public async Task<Response<int>> Handle(SendAppointmentRemindersCommand request, CancellationToken cancellationToken)
        {
            var upcomingAppointments = await appointmentService.CheckUpcomingAppointmentsAsync();
            var sentCount = 0;

            foreach (var appointment in upcomingAppointments)
            {
                var customer = appointment.Booking.Customer;
                var technician = appointment.Booking.Technician.User;
                try
                {
                    var notification = new AppointmentNotification
                    {
                        BookingId = appointment.BookingId,
                        CustomerId = customer.Id,
                        TechnicianId = technician.Id,
                        ScheduledStart = appointment.ScheduledStart,
                        ScheduledEnd = appointment.ScheduledEnd,
                        Notes = appointment.Notes,
                        Title = "New Appointment",
                        Email = customer.Email,
                        Message =
                             $"Your appointment is coming up at " +
                                  $"{appointment.ScheduledStart:dd/MM/yyyy hh:mm tt} " +
                              $"with {technician.FullName}."
                    };


                    await notificationService.SendNotification(notification, "appointment-email-notifications");
                    await appointmentService.MakeReminderSentTrue(appointment.Id);
                    sentCount++;
                }
                catch (Exception ex)
                {
                    loggerService.LogError($"Failed to send reminder for appointment {appointment.Id}: {ex.Message}");
                }
            }
            return Success(sentCount);


        }
    }
}
