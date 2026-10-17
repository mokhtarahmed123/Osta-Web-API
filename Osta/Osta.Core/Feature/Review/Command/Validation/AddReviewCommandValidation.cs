using FluentValidation;
using Osta.Core.Feature.Review.Command.Model;

namespace Osta.Core.Feature.Review.Command.Validation
{
    public class AddReviewCommandValidation : AbstractValidator<AddReviewCommand>
    {
        public AddReviewCommandValidation()
        {
            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage(
                    "Booking ID must be greater than 0.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage(
                    "Rating must be between 1 and 5.");

            RuleFor(x => x.Comment)
                .MaximumLength(1000)
                .WithMessage(
                    "Comment cannot exceed 1000 characters.")
                .When(x =>
                    !string.IsNullOrWhiteSpace(x.Comment));
        }
    }
}
