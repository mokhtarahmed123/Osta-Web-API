using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.MediaBooking.Command.Model;
using Osta.Data.Entities.Identity;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.MediaBooking.Command.Handler
{
    public class UpdateMediaBookingCommandHandler : MediaBookingCommandHandler, IRequestHandler<UpdateMediaBookingCommand, Response<string>>
    {
        public UpdateMediaBookingCommandHandler(IMapper mapper, ILoggerService loggerService, IMediaService mediaService, IBookingService bookingService, ICurrentUserService currentUserService, UserManager<User> userManager) : base(mapper, loggerService, mediaService, bookingService, currentUserService, userManager)
        {
        }

        public async Task<Response<string>> Handle(UpdateMediaBookingCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var customerId = currentUserService.UserId;
            if (string.IsNullOrEmpty(customerId))
                throw new UnauthorizedAccessException("You are not authorized.");

            var media = await mediaService.GetByIdAsync(request.Id, cancellationToken);
            if (media is null)
                return NotFound<string>($"Media with Id {request.Id} was not found.");

            var booking = await bookingService.GetBookingById(media.BookingId);
            if (booking is null)
                return NotFound<string>($"Booking with Id {media.BookingId} was not found.");

            if (booking.CustomerId != customerId)
                return Unauthorized<string>("This media does not belong to you.");

            mapper.Map(request, media);

            try
            {

                await mediaService.UpdateAsync(request.Id, media, request.File, cancellationToken);

            }
            catch (Exception ex)
            {
                return BadRequest<string>($"Failed to update media: {ex.Message}");
            }

            return Success<string>("Media updated successfully.");
        }

    }
}
