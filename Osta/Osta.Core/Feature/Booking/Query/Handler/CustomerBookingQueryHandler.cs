using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.Booking.Query.Model.CustomerModel;
using Osta.Core.Feature.Booking.Query.Result;
using Osta.Data.Entities.Identity;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Booking.Query.Handler
{
    public class CustomerBookingQueryHandler : ResponseHandler,
        IRequestHandler<GetAllBookingsAsCustomerQuery, Response<List<GetAllBookingsAsCustomerResult>>>
    {
        private readonly IMapper mapper;
        private readonly IBookingService bookingService;
        private readonly ILoggerService loggerService;
        private readonly UserManager<User> userManager;
        private readonly ICurrentUserService currentUserService;
        private readonly IBookingServiceService bookingServiceService;

        public CustomerBookingQueryHandler(IMapper mapper, IBookingService bookingService, ILoggerService loggerService,
            UserManager<User> userManager, ICurrentUserService currentUserService, IBookingServiceService bookingServiceService)
        {
            this.mapper = mapper;
            this.bookingService = bookingService;
            this.loggerService = loggerService;
            this.userManager = userManager;
            this.currentUserService = currentUserService;
            this.bookingServiceService = bookingServiceService;
        }
        public async Task<Response<List<GetAllBookingsAsCustomerResult>>> Handle(GetAllBookingsAsCustomerQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var customerId = currentUserService.UserId;

            if (string.IsNullOrEmpty(customerId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            var bookings =
                await bookingService.GetBookingsByClientId(customerId);

            var result =
                mapper.Map<List<GetAllBookingsAsCustomerResult>>(bookings);
            return Success(result, $"Count => {bookings.Count()}");
        }
    }
}
