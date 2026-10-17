using FluentValidation;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;

namespace Osta.Core.Feature.Authentication.Command.Validation.AuthValidation
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                           .NotEmpty()
                           .WithMessage("Email is required.")
                           .EmailAddress()
                           .WithMessage("Invalid email address.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm password is required.")
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");
        }
    }
}
