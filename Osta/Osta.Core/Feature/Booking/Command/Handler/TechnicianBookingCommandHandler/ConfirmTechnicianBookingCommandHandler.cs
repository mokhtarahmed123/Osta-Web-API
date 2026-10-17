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

namespace Osta.Core.Feature.Booking.Command.Handler.TechnicianBookingCommandHandler
{
    public class ConfirmTechnicianBookingCommandHandler : ResponseHandler, IRequestHandler<ConfirmBookingCommand, Response<string>>
    {

        private readonly UserManager<User> userManager;
        private readonly IBookingService bookingService;
        private readonly ICurrentUserService currentUserService;

        private readonly ISendNotificationMessage sendNotificationMessage;


        public ConfirmTechnicianBookingCommandHandler(UserManager<User> userManager, IBookingService bookingService,
            ICurrentUserService currentUserService, ISendNotificationMessage sendNotificationMessage
                )
        {
            this.userManager = userManager;
            this.bookingService = bookingService;
            this.currentUserService = currentUserService;
            this.sendNotificationMessage = sendNotificationMessage;

        }
        public async Task<Response<string>> Handle(
  ConfirmBookingCommand request,
  CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.BookingId <= 0)
                return BadRequest<string>("Booking Id must be greater than 0.");

            var technicianId = currentUserService.UserId;

            if (string.IsNullOrEmpty(technicianId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            await bookingService.ConfirmBooking(
                request.BookingId);
            // If He Accept 
            var booking = await bookingService.GetBookingById(request.BookingId);

            var user = await userManager.FindByIdAsync(booking.CustomerId);



            var notification = new BookingNotification
            {
                BookingId = request.BookingId,
                TechnicianId = technicianId,
                CustomerId = user.Id,
                Email = user.Email,
                Status = BookingStatus.Confirmed.ToString(),
                Title = $"Booking Confirmed - #{booking.Id}",
                Message =
            $"Your booking request has been accepted successfully. " +
            $"Booking number: {booking.Id}. " +
            $"Please wait while the technician selects an appointment time."
            };

            await sendNotificationMessage.SendNotification(notification, "Booking");

            return Success(
                "Booking confirmed successfully. Please wait to select an appointment.");
        }


    }
}
