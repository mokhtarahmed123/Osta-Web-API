using FluentValidation;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;

namespace Osta.Core.Feature.Authentication.Command.Validation.AuthValidation
{
    public class SendResetPasswordCommandValidator : AbstractValidator<SendResetPasswordCommand>
    {
        public SendResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email address.");
        }
    }
}
