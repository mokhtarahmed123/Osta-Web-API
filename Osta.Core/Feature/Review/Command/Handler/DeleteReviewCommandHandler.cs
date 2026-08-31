using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.Review.Command.Model;
using Osta.Data.Entities.Identity;
using Osta.Service.Abstract.ReviewAbstract;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Review.Command.Handler
{
    public class DeleteReviewCommandHandler : ResponseHandler, IRequestHandler<DeleteReviewCommand, Response<string>>
    {

        private readonly ICurrentUserService currentUserService;
        private readonly IReviewService reviewService;
        private readonly IBookingService bookingService;
        private readonly UserManager<User> userManager;
        private readonly ITechnicianService technicianService;

        public DeleteReviewCommandHandler(ICurrentUserService currentUserService, IReviewService reviewService, IBookingService bookingService, UserManager<User> userManager, ITechnicianService technicianService)
        {

            this.currentUserService = currentUserService;
            this.reviewService = reviewService;
            this.bookingService = bookingService;
            this.userManager = userManager;
            this.technicianService = technicianService;
        }
        public async Task<Response<string>> Handle(
       DeleteReviewCommand request,
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
                    "Only customers can Delete reviews.");

            var review =
                await reviewService.GetReview(
                    request.Id,
                    cancellationToken);

            if (review is null)
                return NotFound<string>(
                    "Review not found.");

            var booking =
                await bookingService.GetBookingById(
                    review.BookingId);

            if (booking is null)
                return NotFound<string>(
                    "Booking not found.");

            if (booking.CustomerId != customerId)
                return Unauthorized<string>(
                    "You cannot delete this review.");

            await reviewService.Delete(
                request.Id,
                cancellationToken);
            await technicianService.UpdateReviewCount(booking.TechnicianId, -1);


            return Success<string>(
                "Review deleted successfully.");
        }

    }
}
