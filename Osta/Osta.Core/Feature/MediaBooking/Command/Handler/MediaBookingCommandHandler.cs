using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Data.Entities.Identity;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.MediaBooking.Command.Handler
{
    public class MediaBookingCommandHandler : ResponseHandler



    {
        protected readonly IMapper mapper;
        protected readonly ILoggerService loggerService;
        protected readonly IMediaService mediaService;
        protected readonly IBookingService bookingService;
        protected readonly ICurrentUserService currentUserService;
        protected readonly UserManager<User> userManager;

        public MediaBookingCommandHandler(IMapper mapper, ILoggerService loggerService, IMediaService mediaService, IBookingService bookingService, ICurrentUserService currentUserService, UserManager<User> userManager)

        {
            this.mapper = mapper;
            this.loggerService = loggerService;
            this.mediaService = mediaService;
            this.bookingService = bookingService;
            this.currentUserService = currentUserService;
            this.userManager = userManager;
        }




    }
}
