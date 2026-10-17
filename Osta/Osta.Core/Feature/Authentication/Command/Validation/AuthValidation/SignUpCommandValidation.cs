using FluentValidation;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;

namespace Osta.Core.Feature.Authentication.Command.Validation.AuthValidation
{
    public class SignUpCommandValidation : AbstractValidator<SignUpCommand>
    {
        public SignUpCommandValidation()
        {
            RuleFor(x => x.FullName)
           .NotEmpty()
           .WithMessage("Full name is required.")
           .MinimumLength(3)
           .WithMessage("Full name must be at least 3 characters.")
           .MaximumLength(100)
           .WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email address.")
                .MaximumLength(256)
                .WithMessage("Email must not exceed 256 characters.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.")
                .MaximumLength(100)
                .WithMessage("Password must not exceed 100 characters.")
                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]")
                .WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm password is required.")
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^01[0125][0-9]{8}$")
                .WithMessage("Invalid Egyptian phone number.");
        }
    }
}
