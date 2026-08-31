using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.MediaBooking.Command.Model;
using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.MediaBooking.Command.Handler
{
    public class AddMediaBookingCommandHandler : MediaBookingCommandHandler, IRequestHandler<AddMediaBookingCommand, Response<string>>
    {
        public AddMediaBookingCommandHandler(IMapper mapper, ILoggerService loggerService, IMediaService mediaService, IBookingService bookingService, ICurrentUserService currentUserService, UserManager<User> userManager) : base(mapper, loggerService, mediaService, bookingService, currentUserService, userManager)
        {
        }

        public async Task<Response<string>> Handle(AddMediaBookingCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var customerId = currentUserService.UserId;
            if (string.IsNullOrEmpty(customerId))
                throw new UnauthorizedAccessException("You are not authorized.");

            if (request.File is null || request.File.Length == 0)
                return BadRequest<string>("No file was provided.");

            var booking = await bookingService.GetBookingById(request.BookingId);
            if (booking is null)
                return NotFound<string>($"Booking with Id {request.BookingId} was not found.");

            if (booking.CustomerId != customerId)
                return Unauthorized<string>("This booking does not belong to you.");

            var mediaBooking = mapper.Map<Media>(request);
            mediaBooking.UploadedByUserId = customerId;

            try
            {
                await mediaService.AddAsync(mediaBooking, request.File, cancellationToken);
            }
            catch (Exception ex)
            {

                return BadRequest<string>($"Failed to upload media: {ex.Message}");
            }

            return Success<string>("Media added successfully.");
        }
    }
}
