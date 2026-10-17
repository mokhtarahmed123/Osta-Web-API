using FluentValidation;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Domain.Enum;

namespace Osta.Core.Feature.Coupon.Command.Validation
{
    public class UpdateCouponCommandValidator : AbstractValidator<UpdateCouponCommand>
    {
        public UpdateCouponCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Invalid coupon Id.");

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

            RuleFor(x => x.UsageLimit)
                .GreaterThan(0)
                .WithMessage("Usage limit must be at least 1.");
        }
    }
}