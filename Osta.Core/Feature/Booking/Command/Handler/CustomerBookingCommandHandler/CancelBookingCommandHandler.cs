using MediatR;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.Booking.Command.Model.CustomerModel;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Booking.Command.Handler.CustomerBookingCommandHandler
{
    public class CancelBookingCommandHandler : ResponseHandler, IRequestHandler<CancelBookingCommand, Response<string>>
    {

        private readonly IBookingService bookingService;
        private readonly ILoggerService loggerService;
        private readonly ICurrentUserService currentUserService;
        private readonly IBookingServiceService bookingServiceService;

        public CancelBookingCommandHandler(IBookingService bookingService, ILoggerService loggerService, ICurrentUserService currentUserService, IBookingServiceService bookingServiceService)
        {

            this.bookingService = bookingService;
            this.loggerService = loggerService;
            this.currentUserService = currentUserService;
            this.bookingServiceService = bookingServiceService;
        }

        public async Task<Response<string>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.BookingId <= 0)
                return BadRequest<string>("Booking Id must be greater than 0.");

            var technicianId = currentUserService.UserId;

            if (string.IsNullOrEmpty(technicianId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            await bookingService.CancelBooking(
                request.BookingId);

            await bookingServiceService.DeleteRangeByBookingId(
                request.BookingId);

            loggerService.LogInformation(
                $"All services associated with Booking Id {request.BookingId} " +
                $"were deleted because the booking was canceled.");


            return Success(
                "Booking Canceled successfully.");

        }

    }
}
