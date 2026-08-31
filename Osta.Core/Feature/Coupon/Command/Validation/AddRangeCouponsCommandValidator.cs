using FluentValidation;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Domain.Enum;

namespace Osta.Core.Feature.Coupon.Command.Validation
{
    public class AddRangeCouponsCommandValidator : AbstractValidator<AddRangeCouponsCommand>
    {
        public AddRangeCouponsCommandValidator()
        {
            RuleFor(x => x.Count)
                .GreaterThan(0)
                .LessThanOrEqualTo(500)
                .WithMessage("Count must be between 1 and 500.");

            RuleFor(x => x.DiscountType)
                .IsInEnum()
                .WithMessage("Invalid discount type.");

            RuleFor(x => x.DiscountValue)
                .GreaterThan(0)
                .WithMessage("Discount value must be greater than 0.");

            RuleFor(x => x.DiscountValue)
                .LessThanOrEqualTo(100)
                .When(x => x.DiscountType == DiscountTypeEnum.Percentage)
                .WithMessage("Percentage discount cannot exceed 100.");

            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Start date cannot be in the past.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date.");

            RuleFor(x => x.UsageLimit)
                .GreaterThan(0)
                .WithMessage("Usage limit must be at least 1.");
        }
    }
}