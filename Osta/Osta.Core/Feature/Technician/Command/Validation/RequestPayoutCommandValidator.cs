using FluentValidation;
using Osta.Core.Feature.Technician.Command.Model.TechnicianPayout;
using Osta.Domain.Entities.Technician;

namespace Osta.Core.Feature.Technician.Command.Validation.TechnicianPayout
{
    public class RequestPayoutCommandValidator : AbstractValidator<RequestPayoutCommand>
    {
        public RequestPayoutCommandValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Payout amount must be greater than zero.");

            RuleFor(x => x.Method)
                .IsInEnum()
                .WithMessage("Invalid payout method.");

            RuleFor(x => x.ReceivingDetails)
                .NotEmpty()
                .WithMessage("Receiving details are required.");

            RuleFor(x => x.ReceivingDetails)
                .Matches(@"^01[0125][0-9]{8}$")
                .When(x => x.Method == PayoutMethod.VodafoneCash || x.Method == PayoutMethod.InstaPay)
                .WithMessage("Invalid Egyptian phone number.");
        }
    }
}