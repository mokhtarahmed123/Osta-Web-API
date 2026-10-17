using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.Review.Command.Model;
using Osta.Data.Entities.Identity;
using Osta.Data.Enum;
using Osta.Service.Abstract.ReviewAbstract;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Review.Command.Handler
{
    public class AddReviewCommandHandler : ResponseHandler, IRequestHandler<AddReviewCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUserService;
        private readonly IReviewService reviewService;
        private readonly IBookingService bookingService;
        private readonly UserManager<User> userManager;
        private readonly ITechnicianService technicianService;

        public AddReviewCommandHandler(IMapper mapper, ICurrentUserService currentUserService,
            IReviewService reviewService, IBookingService bookingService,
            UserManager<User> userManager, ITechnicianService technicianService)
        {
            this.mapper = mapper;
            this.currentUserService = currentUserService;
            this.reviewService = reviewService;
            this.bookingService = bookingService;
            this.userManager = userManager;
            this.technicianService = technicianService;
        }
        public async Task<Response<string>> Handle(
         AddReviewCommand request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var customerId = currentUserService.UserId;

            if (string.IsNullOrEmpty(customerId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");


            var user = await userManager.FindByIdAsync(customerId);

            if (user is null)
                return NotFound<string>(
                    "User not found.");

            var roles = await userManager.GetRolesAsync(user);

            if (!roles.Contains("User"))
                return Unauthorized<string>(
                    "Only User can create reviews.");

            // Get Booking
            var booking =
                await bookingService.GetBookingById(
                    request.BookingId);

            if (booking is null)
                return NotFound<string>(
                    "Booking not found.");

            // Check ownership
            if (booking.CustomerId != customerId)
                return Unauthorized<string>(
                    "This booking does not belong to you.");

            // Only completed bookings can be reviewed
            if (booking.Status != BookingStatus.Completed)
                return BadRequest<string>(
                    "You can review only completed bookings.");

            // Check if already reviewed
            var existingReview =
                await reviewService.GetByBookingId(
                    request.BookingId,
                    cancellationToken);

            if (existingReview is not null)
                return BadRequest<string>(
                    "You have already reviewed this booking.");

            var review =
               mapper.Map<Osta.Data.Entities.Review>(request);

            await reviewService.Add(
                review,
                cancellationToken);

            await technicianService.UpdateReviewCount(booking.TechnicianId, +1);


            await technicianService.RateTechnicianAsync(
                booking.TechnicianId,
                cancellationToken);

            return Success<string>(
                "Review added successfully.");
        }

    }
}
