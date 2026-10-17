using FluentValidation;
using Osta.Core.Feature.Appointment.Command.Model;

namespace Osta.Core.Feature.Appointment.Command.Validation
{
    public class UpdateAppointmentCommandValidation : AbstractValidator<UpdateAppointmentCommand>
    {
        public UpdateAppointmentCommandValidation()
        {


            RuleFor(x => x.ScheduledStart)
                .NotEmpty()
                .WithMessage("Scheduled start is required.")
                .GreaterThan(DateTime.Now)
                .WithMessage("Scheduled start must be in the future.");

            RuleFor(x => x.ScheduledEnd)
                .NotEmpty()
                .WithMessage("Scheduled end is required.")
                .GreaterThan(x => x.ScheduledStart)
                .WithMessage("Scheduled end must be after scheduled start.");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Notes));
        }
    }
}
