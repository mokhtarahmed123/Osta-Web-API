using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.Booking.Command.Model.TechnicianModel;
using Osta.Data.Entities.Identity;
using Osta.Data.Enum;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Booking.Command.Handler.TechnicianBookingCommandHandler
{
    public class RejectTechnicianBookingCommandHandler : ResponseHandler, IRequestHandler<RejectBookingCommand, Response<string>>

    {

        private readonly UserManager<User> userManager;
        private readonly IBookingService bookingService;
        private readonly ICurrentUserService currentUserService;

        private readonly ISendNotificationMessage sendNotificationMessage;

        private readonly ILoggerService loggerService;

        public RejectTechnicianBookingCommandHandler(UserManager<User> userManager, IBookingService bookingService, ICurrentUserService currentUserService, ISendNotificationMessage sendNotificationMessage, ILoggerService loggerService)
        {

            this.userManager = userManager;
            this.bookingService = bookingService;
            this.currentUserService = currentUserService;

            this.sendNotificationMessage = sendNotificationMessage;

            this.loggerService = loggerService;
        }

        public async Task<Response<string>> Handle(
            RejectBookingCommand request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.BookingId <= 0)
                return BadRequest<string>(
                    "Booking Id must be greater than 0.");

            var technicianId = currentUserService.UserId;

            if (string.IsNullOrEmpty(technicianId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            var booking =
                await bookingService.GetBookingById(
                    request.BookingId);

            if (booking is null)
                return NotFound<string>(
                    "Booking not found.");

            if (booking.TechnicianId != technicianId)
                return Unauthorized<string>(
                    "This booking does not belong to you.");

            if (booking.Status != BookingStatus.Pending)
                return BadRequest<string>(
                    "Only pending bookings can be rejected.");

            await bookingService.RefuseBooking(
                request.BookingId);

            var user = await userManager.FindByIdAsync(booking.CustomerId);



            var notification = new BookingNotification
            {
                BookingId = request.BookingId,
                TechnicianId = technicianId,
                CustomerId = user.Id,
                Email = user.Email,
                Status = BookingStatus.Refused.ToString(),
                Title = $"Refuse Booking With Number {booking.Id} ",
                Message =
              $"Unfortunately, the technician was unable to accept your booking request. " +
              $"Booking number: {booking.Id}."

            };
            await sendNotificationMessage.SendNotification(notification, "Booking");





            loggerService.LogInformation(
                $"Booking Id {request.BookingId} was rejected by " +
                $"Technician Id {technicianId}. " +
                $"Booking services were preserved for history.");

            return Success(
                "Booking refused successfully.");
        }


    }
}
