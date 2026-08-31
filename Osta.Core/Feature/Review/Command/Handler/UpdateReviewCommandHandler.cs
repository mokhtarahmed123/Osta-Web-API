using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Booking.Interface;
using Osta.Core.Bases;
using Osta.Core.Feature.Review.Command.Model;
using Osta.Data.Entities.Identity;
using Osta.Service.Abstract.ReviewAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Review.Command.Handler
{
    public class UpdateReviewCommandHandler : ResponseHandler, IRequestHandler<UpdateReviewCommand, Response<string>>

    {

        private readonly ICurrentUserService currentUserService;
        private readonly IReviewService reviewService;
        private readonly IBookingService bookingService;
        private readonly UserManager<User> userManager;


        public UpdateReviewCommandHandler(ICurrentUserService currentUserService, IReviewService reviewService, IBookingService bookingService, UserManager<User> userManager)
        {

            this.currentUserService = currentUserService;
            this.reviewService = reviewService;
            this.bookingService = bookingService;
            this.userManager = userManager;

        }

        public async Task<Response<string>> Handle(
            UpdateReviewCommand request,
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
                    "Only customers can Update reviews.");

            // Get Review
            var review =
                await reviewService.GetReview(
                    request.Id,
                    cancellationToken);

            if (review is null)
                return NotFound<string>(
                    "Review not found.");

            // Get Booking
            var booking =
                await bookingService.GetBookingById(
                    review.BookingId);

            if (booking is null)
                return NotFound<string>(
                    "Booking not found.");

            // Check ownership
            if (booking.CustomerId != customerId)
                return Unauthorized<string>(
                    "You cannot update this review.");

            // Update only Rating + Comment
            review.Rating = request.Rating;
            review.Comment = request.Comment;

            await reviewService.Update(
                request.Id,
                review,
                cancellationToken);

            return Success<string>(
                "Review updated successfully.");
        }
    }
}
