using FluentValidation;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Domain.Enum;

namespace Osta.Core.Feature.Coupon.Command.Validation
{
    public class AddCouponCommandValidator : AbstractValidator<AddCouponCommand>
    {
        public AddCouponCommandValidator()
        {
            RuleFor(x => x.Code)
              .MaximumLength(20)
              .MinimumLength(4)
              .When(x => !string.IsNullOrWhiteSpace(x.Code))
              .WithMessage("Coupon code must be between 4 and 20 characters.");

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
                .NotEmpty()
                .WithMessage("Start date is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("End date is required.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date.");

            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Start date cannot be in the past.");

            RuleFor(x => x.UsageLimit)
                .GreaterThan(0)
                .WithMessage("Usage limit must be at least 1.");
        }
    }
}
