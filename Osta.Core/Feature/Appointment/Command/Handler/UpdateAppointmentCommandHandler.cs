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
    public class UpdateAppointmentCommandHandler : ResponseHandler, IRequestHandler<UpdateAppointmentCommand, Response<string>>
    {

        private readonly IAppointmentService appointmentService;
        private readonly UserManager<User> userManager;
        private readonly ICurrentUserService currentUserService;
        private readonly IBookingService bookingService;
        private readonly ISendNotificationMessage sendNotificationMessage;

        public UpdateAppointmentCommandHandler(IAppointmentService appointmentService, UserManager<User> userManager,
            ICurrentUserService currentUserService, IBookingService bookingService, ISendNotificationMessage sendNotificationMessage)

        {

            this.appointmentService = appointmentService;
            this.userManager = userManager;
            this.currentUserService = currentUserService;
            this.bookingService = bookingService;
            this.sendNotificationMessage = sendNotificationMessage;
        }
        public async Task<Response<string>> Handle(
         UpdateAppointmentCommand request,
         CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.AppointmentId))
                return BadRequest<string>(
                    "Appointment Id is required.");

            if (request.BookingId <= 0)
                return BadRequest<string>(
                    "Booking Id must be greater than 0.");

            var technicianId = currentUserService.UserId;

            if (string.IsNullOrEmpty(technicianId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            // Get Appointment
            var appointment =
                await appointmentService.Get(
                    request.AppointmentId,
                    cancellationToken);

            if (appointment is null)
                return NotFound<string>(
                    "Appointment not found.");

            // Get Booking
            var booking =
                await bookingService.GetBookingById(
                    request.BookingId);

            if (booking is null)
                return NotFound<string>(
                    "Booking not found.");

            // Make sure booking belongs to current technician
            if (booking.TechnicianId != technicianId)
                return Unauthorized<string>(
                    "This booking does not belong to you.");

            // Make sure appointment belongs to this booking
            if (appointment.BookingId != request.BookingId)
                return BadRequest<string>(
                    "This appointment does not belong to this booking.");

            // Check appointment conflict
            var hasConflict =
                await appointmentService.HasConflictAsync(
                    request.AppointmentId,
                    technicianId,
                    request.ScheduledStart,
                    request.ScheduledEnd
            );

            if (hasConflict)
            {
                return BadRequest<string>(
                    "You already have another appointment during this time.");
            }

            // Update
            appointment.ScheduledStart = request.ScheduledStart;
            appointment.ScheduledEnd = request.ScheduledEnd;
            appointment.Notes = request.Notes;
            appointment.IsApproved = false;


            await appointmentService.UpdateAppointmentAsync(
                request.AppointmentId,
                appointment,
                cancellationToken);
            var customer =
    await userManager.FindByIdAsync(booking.CustomerId);

            if (customer is null)
                return NotFound<string>("Customer not found.");
            var notification = new AppointmentNotification
            {
                BookingId = booking.Id,
                CustomerId = booking.CustomerId,
                TechnicianId = technicianId,

                ScheduledStart = request.ScheduledStart,
                ScheduledEnd = request.ScheduledEnd,

                Notes = request.Notes,
                Email = customer.Email,

                Title = "Appointment Updated",

                Message =
        $"Your appointment has been updated. " +
        $"New time: {request.ScheduledStart:dd/MM/yyyy hh:mm tt} " +
        $"to {request.ScheduledEnd:hh:mm tt}."
            };

            await sendNotificationMessage.SendNotification(notification, "appointment-email-notifications");

            return Success<string>(
                "Appointment updated successfully.");
        }
    }
}
