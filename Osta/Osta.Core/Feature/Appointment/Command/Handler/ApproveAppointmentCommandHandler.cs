using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.Appointment.Command.Model;
using Osta.Data.Entities.Identity;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.AppointmentAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Appointment.Command.Handler
{
    public class ApproveAppointmentCommandHandler : ResponseHandler, IRequestHandler<ApproveAppointmentCommand, Response<string>>
    {

        private readonly IAppointmentService appointmentService;
        private readonly UserManager<User> userManager;
        private readonly ICurrentUserService currentUserService;
        private readonly IBookingService bookingService;
        private readonly ISendNotificationMessage sendNotificationMessage;

        public ApproveAppointmentCommandHandler(IAppointmentService appointmentService, UserManager<User> userManager, ICurrentUserService currentUserService, IBookingService bookingService, ISendNotificationMessage sendNotificationMessage)
        {

            this.appointmentService = appointmentService;
            this.userManager = userManager;
            this.currentUserService = currentUserService;
            this.bookingService = bookingService;
            this.sendNotificationMessage = sendNotificationMessage;
        }

        public async Task<Response<string>> Handle(
    ApproveAppointmentCommand request,
    CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var customerId = currentUserService.UserId;

            if (string.IsNullOrEmpty(customerId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            var appointment =
                await appointmentService.Get(
                    request.AppointmentId,
                    cancellationToken);

            if (appointment is null)
                return NotFound<string>("Appointment not found.");

            var booking =
                await bookingService.GetBookingById(
                    appointment.BookingId);

            if (booking is null)
                return NotFound<string>("Booking not found.");

            if (booking.CustomerId != customerId)
                return Unauthorized<string>(
                    "This appointment does not belong to you.");

            if (appointment.IsApproved)
                return BadRequest<string>(
                    "Appointment is already approved.");

            await appointmentService.ApproveAppointmentAsync(
                request.AppointmentId);

            var technician =
    await userManager.FindByIdAsync(booking.TechnicianId);

            if (technician is null)
                return NotFound<string>("Technician not found.");

            var notification = new AppointmentNotification
            {
                BookingId = booking.Id,

                CustomerId = booking.CustomerId,
                TechnicianId = booking.TechnicianId,
                Email = technician.Email,
                ScheduledStart = appointment.ScheduledStart,
                ScheduledEnd = appointment.ScheduledEnd,

                Notes = appointment.Notes,

                Title = "Appointment Approved",

                Message =
         $"The customer has approved the appointment. " +
         $"Appointment time: " +
         $"{appointment.ScheduledStart:dd/MM/yyyy hh:mm tt} " +
         $"to {appointment.ScheduledEnd:hh:mm tt}."
            };

            await sendNotificationMessage.SendNotification(
                notification,
                "appointment-email-notifications");

            return Success<string>(
                "Appointment approved successfully.");
        }
    }
}
