using FluentValidation;
using Osta.Core.Feature.ServiceArea.Command.Model;

namespace Osta.Core.Feature.ServiceArea.Command.Validation
{
    public class AddServiceAreaCommandValidation : AbstractValidator<AddServiceAreaCommand>
    {
        public AddServiceAreaCommandValidation()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100).WithMessage("State must not exceed 100 characters.");
        }
    }
}
