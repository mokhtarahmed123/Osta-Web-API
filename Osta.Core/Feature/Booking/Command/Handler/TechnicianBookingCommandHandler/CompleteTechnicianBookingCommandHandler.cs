using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.Booking.Command.Model.TechnicianModel;
using Osta.Data.Entities.Identity;
using Osta.Data.Enum;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Booking.Command.Handler.TechnicianBookingCommandHandler
{
    public class CompleteTechnicianBookingCommandHandler : ResponseHandler, IRequestHandler<CompleteBookingCommand, Response<string>>
    {
        private readonly UserManager<User> userManager;
        private readonly IBookingService bookingService;
        private readonly ICurrentUserService currentUserService;
        private readonly ITechnicianService technicianService;
        private readonly ISendNotificationMessage sendNotificationMessage;

        public CompleteTechnicianBookingCommandHandler(UserManager<User> userManager, IBookingService bookingService,
            ICurrentUserService currentUserService, ITechnicianService technicianService, ISendNotificationMessage sendNotificationMessage

            )
        {
            this.userManager = userManager;
            this.bookingService = bookingService;
            this.currentUserService = currentUserService;
            this.technicianService = technicianService;
            this.sendNotificationMessage = sendNotificationMessage;

        }

        public async Task<Response<string>> Handle(
CompleteBookingCommand request,
CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.Id <= 0)
                return BadRequest<string>(
                    "Booking Id must be greater than 0.");

            var technicianId = currentUserService.UserId;

            if (string.IsNullOrEmpty(technicianId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            var booking =
                await bookingService.GetBookingById(request.Id);

            if (booking is null)
                return NotFound<string>(
                    "Booking not found.");

            if (booking.TechnicianId != technicianId)
                return Unauthorized<string>(
                    "This booking does not belong to you.");

            if (booking.Status == BookingStatus.Completed)
                return BadRequest<string>(
                    "Booking is already completed.");

            if (booking.Status != BookingStatus.Confirmed)
                return BadRequest<string>(
                    "Only confirmed bookings can be completed.");

            // Complete Booking
            await bookingService.CompleteBooking(
                request.Id);

            // Increase technician completed bookings
            await technicianService.CompleteBooking(
                technicianId);

            // Get Customer
            var customer =
                await userManager.FindByIdAsync(
                    booking.CustomerId);

            if (customer is null)
                return NotFound<string>(
                    "Customer not found.");

            // Send notification to customer
            var notification = new NotificationMessage
            {
                RecipientId = customer.Id,
                RecipientEmail = customer.Email,
                BookingId = booking.Id,
                RecipientName = customer.FullName,
                Message =
                    $"Your booking #{booking.Id} has been completed successfully. " +
                    "Please rate the technician and leave a review."
            };

            await sendNotificationMessage.SendNotification(
                notification,
                "Booking");

            return Success(
                "Booking completed successfully.");
        }
    }
}
