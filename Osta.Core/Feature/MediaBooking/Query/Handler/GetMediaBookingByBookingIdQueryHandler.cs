using AutoMapper;
using MediatR;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.MediaBooking.Query.Model;
using Osta.Core.Feature.MediaBooking.Query.Result;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.MediaBooking.Query.Handler
{
    public class GetMediaBookingByBookingIdQueryHandler : MediaBookingQueryHandler, IRequestHandler<
            GetMediaBookingByBookingIdQuery,
            Response<List<GetMediaBookingByBookingIdResult>>>

    {
        public GetMediaBookingByBookingIdQueryHandler(IMediaService mediaService, IBookingService bookingService, IMapper mapper, ICurrentUserService currentUserService) : base(mediaService, bookingService, mapper, currentUserService)
        {
        }

        public async Task<Response<List<GetMediaBookingByBookingIdResult>>> Handle(
            GetMediaBookingByBookingIdQuery request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.BookingId <= 0)
                return BadRequest<List<GetMediaBookingByBookingIdResult>>(
                    "Booking Id must be greater than 0.");

            var userId = currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            var booking = await bookingService.GetBookingById(
                request.BookingId);

            if (booking is null)
                return NotFound<List<GetMediaBookingByBookingIdResult>>(
                    "Booking not found.");

            if (booking.CustomerId != userId &&
                booking.TechnicianId != userId)
                return Unauthorized<List<GetMediaBookingByBookingIdResult>>(
                    "You are not authorized to access this booking.");

            var media = await mediaService.GetByBookingIdAsync(
                request.BookingId,
                cancellationToken);

            if (!media.Any())
                return NotFound<List<GetMediaBookingByBookingIdResult>>(
                    "No media found for this booking.");

            var result =
                mapper.Map<List<GetMediaBookingByBookingIdResult>>(media);

            return Success(result);
        }

    }
}
