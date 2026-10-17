using AutoMapper;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.MediaBooking.Query.Handler
{
    public class MediaBookingQueryHandler : ResponseHandler
    {
        protected readonly IMediaService mediaService;
        protected readonly IBookingService bookingService;
        protected readonly IMapper mapper;
        protected readonly ICurrentUserService currentUserService;

        public MediaBookingQueryHandler(IMediaService mediaService, IBookingService bookingService, IMapper mapper, ICurrentUserService currentUserService)
        {
            this.mediaService = mediaService;
            this.bookingService = bookingService;
            this.mapper = mapper;
            this.currentUserService = currentUserService;
        }

    }
}