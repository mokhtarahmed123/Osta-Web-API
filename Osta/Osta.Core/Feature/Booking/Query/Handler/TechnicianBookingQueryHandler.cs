using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.Booking.Query.Model.TechnicianModel;
using Osta.Core.Feature.Booking.Query.Result;
using Osta.Core.HandlerMiddleware;
using Osta.Data.Entities.Identity;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Exceptions;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Booking.Query.Handler
{
    public class TechnicianBookingQueryHandler : ResponseHandler,
        IRequestHandler<GetAllBookingsAsTechnicianQuery, Response<List<GetAllBookingsAsTechnicianResult>>>,
        IRequestHandler<GetBookingAsTechnicianQuery, Response<GetBookingAsTechnicianResult>>
    {
        private readonly IMapper mapper;
        private readonly ILoggerService loggerService;
        private readonly UserManager<User> userManager;
        private readonly IBookingService bookingService;
        private readonly ICurrentUserService currentUserService;
        private readonly IBookingServiceService bookingServiceService;

        public TechnicianBookingQueryHandler(IMapper mapper, ILoggerService loggerService,
            UserManager<User> userManager, IBookingService bookingService, ICurrentUserService currentUserService, IBookingServiceService bookingServiceService)
        {
            this.mapper = mapper;
            this.loggerService = loggerService;
            this.userManager = userManager;
            this.bookingService = bookingService;
            this.currentUserService = currentUserService;
            this.bookingServiceService = bookingServiceService;
        }
        public async Task<Response<List<GetAllBookingsAsTechnicianResult>>> Handle(
            GetAllBookingsAsTechnicianQuery request,
            CancellationToken cancellationToken)
        {
            var technicianId = currentUserService.UserId;

            if (string.IsNullOrEmpty(technicianId))
                throw new UnauthorizedException("User is not authenticated.");

            var bookings =
                await bookingService.GetBookingsByTechnicianId(technicianId);

            var result =
                mapper.Map<List<GetAllBookingsAsTechnicianResult>>(bookings);

            return Success(result, $" Count : {bookings.Count()}");
        }

        public async Task<Response<GetBookingAsTechnicianResult>> Handle(
        GetBookingAsTechnicianQuery request,
        CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.BookingId <= 0)
                return BadRequest<GetBookingAsTechnicianResult>(
                    "Booking Id must be greater than 0.");

            var booking = await bookingService.GetBookingById(request.BookingId);

            if (booking is null)
                return NotFound<GetBookingAsTechnicianResult>(
                    "Booking not found.");

            var technicianId = currentUserService.UserId;


            if (booking.TechnicianId != technicianId)
                return Unauthorized<GetBookingAsTechnicianResult>(
                    "You are not authorized to view this booking.");

            var result =
                mapper.Map<GetBookingAsTechnicianResult>(booking);

            var customer = await userManager.FindByIdAsync(result.CustomerId);

            if (customer is null)
                throw new NotFoundException("Customer not found.");

            var BookingService = await bookingServiceService.GetByBookingId(result.BookingId);
            var bookingServices =
                await bookingServiceService.GetByBookingId(result.BookingId);

            result.bookingservicerecord = bookingServices
                .Select(x => new Bookingservicerecord
                {
                    BookingId = x.BookingId,
                    Price = x.PriceAtBooking
                })
                .ToList();
            result.CustomerName = customer.FullName;
            return Success(result);
        }
    }
}
