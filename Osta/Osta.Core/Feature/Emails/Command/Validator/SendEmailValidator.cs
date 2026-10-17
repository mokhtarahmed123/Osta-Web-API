using FluentValidation;
using Osta.Core.Feature.Emails.Command.Model;

namespace Osta.Core.Feature.Emails.Command.Validator
{
    public class SendEmailValidator : AbstractValidator<SendEmailCommand>
    {
        public SendEmailValidator()
        {
            RuleFor(x => x.Email)
                 .NotEmpty()
                 .WithMessage("Email is required.")
                 .EmailAddress()
                 .WithMessage("Invalid email address.");

            RuleFor(x => x.Massege)
                .NotEmpty()
                .WithMessage("Message is required.");
        }
    }
}
