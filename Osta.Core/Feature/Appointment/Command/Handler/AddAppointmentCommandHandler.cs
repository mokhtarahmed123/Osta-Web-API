using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.Appointment.Command.Model;
using Osta.Data.Entities.Identity;
using Osta.Data.Enum;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.AppointmentAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Appointment.Command.Handler
{
    public class AddAppointmentCommandHandler : ResponseHandler, IRequestHandler<AddAppointmentCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly IAppointmentService appointmentService;
        private readonly UserManager<User> userManager;
        private readonly ICurrentUserService currentUserService;
        private readonly IBookingService bookingService;
        private readonly ISendNotificationMessage sendNotificationMessage;

        public AddAppointmentCommandHandler(IMapper mapper, IAppointmentService appointmentService, UserManager<User> userManager, ICurrentUserService currentUserService, IBookingService bookingService, ISendNotificationMessage sendNotificationMessage)

        {
            this.mapper = mapper;
            this.appointmentService = appointmentService;
            this.userManager = userManager;
            this.currentUserService = currentUserService;
            this.bookingService = bookingService;
            this.sendNotificationMessage = sendNotificationMessage;
        }

        public async Task<Response<string>> Handle(
      AddAppointmentCommand request,
      CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var technicianId = currentUserService.UserId;

            if (string.IsNullOrEmpty(technicianId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            // Get Booking
            var booking = await bookingService.GetBookingById(
                request.BookingId);

            if (booking is null)
                return NotFound<string>("Booking not found.");

            var hasConflict = await appointmentService.HasConflictAsync(null, technicianId, request.ScheduledStart, request.ScheduledEnd, cancellationToken);

            if (hasConflict)
            {
                return BadRequest<string>(
                    "You already have another appointment during this time.");
            }

            // Check Technician
            if (booking.TechnicianId != technicianId)
                return Unauthorized<string>(
                    "This booking does not belong to you.");

            // Booking must be confirmed
            if (booking.Status != BookingStatus.Confirmed)
                return BadRequest<string>(
                    "You can create an appointment only for a confirmed booking.");

            // Check existing appointment
            var existingAppointment =
                await appointmentService.GetByBookingIdAsync(
                    request.BookingId);

            if (existingAppointment is not null)
                return BadRequest<string>(
                    "An appointment already exists for this booking.");

            // Create Appointment
            var appointment =
                mapper.Map<Osta.Domain.Entities.Appointment.Appointment>(request);

            appointment.BookingId = request.BookingId;
            appointment.IsApproved = false;

            await appointmentService.AddAppointmentAsync(
                appointment,
                cancellationToken);

            // Get Customer
            var customer =
                await userManager.FindByIdAsync(booking.CustomerId);

            if (customer is null)
                return NotFound<string>("Customer not found.");

            // Create Notification
            var notification = new AppointmentNotification
            {
                BookingId = appointment.BookingId,
                CustomerId = booking.CustomerId,
                TechnicianId = booking.TechnicianId,
                ScheduledStart = appointment.ScheduledStart,
                ScheduledEnd = appointment.ScheduledEnd,
                Notes = appointment.Notes,
                Email = customer.Email,
                Title = "New Appointment",
                Message = $"Your technician scheduled an appointment " +
                          $"from {appointment.ScheduledStart:g} " +
                          $"to {appointment.ScheduledEnd:g}."
            };
            // Send Notification to RabbitMQ
            await sendNotificationMessage.SendNotification(notification, "appointment-email-notifications");


            return Success<string>(
                "Appointment created successfully. " +
                "Customer has been notified.");
        }
    }
}
