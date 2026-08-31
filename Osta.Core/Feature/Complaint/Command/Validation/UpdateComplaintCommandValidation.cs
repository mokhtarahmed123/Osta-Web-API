using FluentValidation;
using Osta.Core.Feature.Complaint.Command.Model;

namespace Osta.Core.Feature.Complaint.Command.Validation
{
    public class UpdateComplaintCommandValidation : AbstractValidator<UpdateComplaintCommand>
    {
        public UpdateComplaintCommandValidation()
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage(
                    "Complaint description is required.")
                .MaximumLength(2000)
                .WithMessage(
                    "Complaint description cannot exceed 2000 characters.");
        }
    }
}
