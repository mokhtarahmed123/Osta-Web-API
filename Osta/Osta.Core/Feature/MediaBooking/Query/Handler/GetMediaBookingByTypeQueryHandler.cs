using AutoMapper;
using MediatR;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.MediaBooking.Query.Model;
using Osta.Core.Feature.MediaBooking.Query.Result;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.MediaBooking.Query.Handler
{
    public class GetMediaBookingByTypeQueryHandler : MediaBookingQueryHandler, IRequestHandler<
            GetMediaBookingByTypeQuery,
            Response<List<GetMediaBookingByTypeResult>>>

    {
        public GetMediaBookingByTypeQueryHandler(IMediaService mediaService, IBookingService bookingService, IMapper mapper, ICurrentUserService currentUserService) : base(mediaService, bookingService, mapper, currentUserService)
        {
        }

        public async Task<Response<List<GetMediaBookingByTypeResult>>> Handle(
            GetMediaBookingByTypeQuery request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.BookingId <= 0)
                return BadRequest<List<GetMediaBookingByTypeResult>>(
                    "Booking Id must be greater than 0.");

            var userId = currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            var booking = await bookingService.GetBookingById(
                request.BookingId);

            if (booking is null)
                return NotFound<List<GetMediaBookingByTypeResult>>(
                    "Booking not found.");

            if (booking.CustomerId != userId &&
                booking.TechnicianId != userId)
                return Unauthorized<List<GetMediaBookingByTypeResult>>(
                    "You are not authorized to access this booking.");

            var media = await mediaService.GetByTypeAsync(
                request.BookingId,
                request.repairType,
                cancellationToken);

            if (!media.Any())
                return NotFound<List<GetMediaBookingByTypeResult>>(
                    "No media found for this booking and type.");

            var result =
                mapper.Map<List<GetMediaBookingByTypeResult>>(media);

            return Success(result);
        }
    }
}
