using AutoMapper;
using MediatR;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.MediaBooking.Query.Model;
using Osta.Core.Feature.MediaBooking.Query.Result;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.MediaBooking.Query.Handler
{
    public class GetMediaBookingByIdQueryHandler : MediaBookingQueryHandler, IRequestHandler<
            GetMediaBookingByIdQuery,
            Response<GetMediaBookingByIdResult>>

    {
        public GetMediaBookingByIdQueryHandler(IMediaService mediaService, IBookingService bookingService, IMapper mapper, ICurrentUserService currentUserService) : base(mediaService, bookingService, mapper, currentUserService)
        {
        }

        public async Task<Response<GetMediaBookingByIdResult>> Handle(
            GetMediaBookingByIdQuery request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.Id <= 0)
                return BadRequest<GetMediaBookingByIdResult>(
                    "Media Id must be greater than 0.");

            var userId = currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            var media = await mediaService.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (media is null)
                return NotFound<GetMediaBookingByIdResult>(
                    "Media not found.");

            var booking = await bookingService.GetBookingById(
                media.BookingId);

            if (booking is null)
                return NotFound<GetMediaBookingByIdResult>(
                    "Booking not found.");

            if (booking.CustomerId != userId &&
                booking.TechnicianId != userId)
                return Unauthorized<GetMediaBookingByIdResult>(
                    "You are not authorized to access this media.");

            var result =
                mapper.Map<GetMediaBookingByIdResult>(media);

            return Success(result);
        }
    }
}
