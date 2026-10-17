using FluentValidation;
using Osta.Core.Feature.Complaint.Command.Model;

namespace Osta.Core.Feature.Complaint.Command.Validation
{
    public class AddComplaintCommandValidation
        : AbstractValidator<AddComplaintCommand>
    {
        public AddComplaintCommandValidation()
        {
            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage(
                    "Booking ID must be greater than 0.");

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