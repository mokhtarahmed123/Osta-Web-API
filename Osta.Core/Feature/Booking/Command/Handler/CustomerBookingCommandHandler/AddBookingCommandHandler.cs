using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Booking.Model;
using Osta.Booking.Producer;
using Osta.Core.Bases;
using Osta.Core.Feature.Booking.Command.Model.CustomerModel;
using Osta.Core.HandlerMiddleware;
using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Data.Enum;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Booking.Command.Handler.CustomerBookingCommandHandler
{
    public class AddBookingCommandHandler : ResponseHandler, IRequestHandler<AddBookingCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly IBookingService bookingService;
        private readonly ILoggerService loggerService;
        private readonly ISendBookingMessage sendBookingMessage;
        private readonly ICurrentUserService currentUserService;
        private readonly UserManager<User> userManager;
        private readonly IServiceService serviceService;

        private readonly ISendNotificationMessage sendNotificationMessage;

        private readonly IBookingServiceService bookingServiceService;

        public AddBookingCommandHandler(IMapper mapper, IBookingService bookingService, ILoggerService loggerService,
            ISendBookingMessage sendBookingMessage, ICurrentUserService currentUserService, UserManager<User> userManager,
            IServiceService serviceService, ISendNotificationMessage sendNotificationMessage,
                 IBookingServiceService bookingServiceService)
        {
            this.mapper = mapper;
            this.bookingService = bookingService;
            this.loggerService = loggerService;
            this.sendBookingMessage = sendBookingMessage;
            this.currentUserService = currentUserService;
            this.userManager = userManager;
            this.serviceService = serviceService;
            this.sendNotificationMessage = sendNotificationMessage;

            this.bookingServiceService = bookingServiceService;
        }

        public async Task<Response<string>> Handle(
      AddBookingCommand request,
      CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;

            var user = await userManager.FindByIdAsync(userId);

            if (user is null || !user.IsActive)
            {
                throw new NotFoundException(
                    "You must sign up or activate your account.");
            }

            var booking = mapper.Map<Bookings>(request);

            booking.CustomerId = userId;
            booking.BookingDate = DateTime.Now;
            booking.Status = BookingStatus.Pending;

            await bookingService.AddBooking(booking);

            var bookingMessage = new SendBooking
            {
                CustomerId = userId,
                CustomerName = user.FullName,
                TechnicianId = request.TechnicianId,
                ServiceId = request.ServiceId,
                City = request.City,
                Area = request.Area,
                BookingDate = booking.BookingDate,
                Governorate = request.Governorate,
                Street = request.Street,
                BookingStatus = BookingStatus.Pending.ToString()
            };

            await sendBookingMessage.SendBooking(
                bookingMessage,
                "Booking");

            var technician = await userManager.FindByIdAsync(request.TechnicianId);
            var notificationMessage = new NotificationMessage
            {
                RecipientId = request.TechnicianId,
                RecipientEmail = technician.Email,
                BookingId = booking.Id,
                RecipientName = user.FullName,
                Message = "You have a new booking request."
            };
            await sendNotificationMessage.SendNotification(notificationMessage, "Notification");

            var service = await serviceService.GetServiceAsync(bookingMessage.ServiceId);
            var bookingserviceManyToMany = new Data.Entities.BookingService
            {
                BookingId = booking.Id,
                PriceAtBooking = service.Price,
                ServiceId = bookingMessage.ServiceId
            };
            loggerService.LogInformation(
                $"Service with Id {bookingserviceManyToMany.ServiceId} " +
                $"was added to Booking with Id {bookingserviceManyToMany.BookingId} " +
                $"successfully with Price {bookingserviceManyToMany.PriceAtBooking}.");
            return new Response<string>
            {
                Data = "Booking created successfully."
            };
        }

    }
}
