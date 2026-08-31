using FluentValidation;
using Osta.Core.Feature.Review.Command.Model;

namespace Osta.Core.Feature.Review.Command.Validation
{
    public class UpdateReviewCommandValidation : AbstractValidator<UpdateReviewCommand>
    {
        public UpdateReviewCommandValidation()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(
                    "Review ID must be greater than 0.");

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
