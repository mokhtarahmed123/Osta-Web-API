using FluentValidation;
using Osta.Core.Feature.Coupon.Command.Model;

namespace Osta.Core.Feature.Coupon.Command.Validation
{
    public class ApplyCouponCommandValidator : AbstractValidator<ApplyCouponCommand>
    {
        public ApplyCouponCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Coupon code is required.");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User Id is required.");

            RuleFor(x => x.OriginalAmount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0.");
        }
    }
}