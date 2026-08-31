using FluentValidation;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianAvailabilities;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Technician.Command.Validation.ValidationTechnicianAvailabilities
{
    public class AddTechnicianAvailabilityCommandValidation
        : AbstractValidator<RequestTechnicianAvailabilityCommand>
    {
        private readonly ICurrentUserService currentUser;

        public AddTechnicianAvailabilityCommandValidation(
            ITechnicianAvailabilityService technicianAvailabilityService, ICurrentUserService currentUser)
        {
            this.currentUser = currentUser;



            RuleFor(x => x.DayOfWeek)
                .IsInEnum()
                .WithMessage("Invalid day of week.");

            RuleFor(x => x)
                .Must(x => x.EndTime > x.StartTime)
                .WithMessage("End time must be greater than start time.");

            RuleFor(x => x.StartTime)
                .Must(time => time >= TimeOnly.MinValue && time <= TimeOnly.MaxValue)
                .WithMessage("Start time is invalid.");



            RuleFor(x => x.EndTime)
                .Must(time => time >= TimeOnly.MinValue && time <= TimeOnly.MaxValue)
                .WithMessage("End time is invalid.");

            RuleFor(x => x)
                .MustAsync(async (model, cancellation) =>
                {
                    var availability = new TechnicianAvailability
                    {
                        TechnicianId = currentUser.UserId,
                        DayOfWeek = model.DayOfWeek,
                        StartTime = model.StartTime,
                        EndTime = model.EndTime,
                        IsAvailable = model.IsAvailable
                    };

                    return !await technicianAvailabilityService.HasOverlappingAvailabilityAsync(availability);
                })
                .WithMessage("This availability overlaps with an existing availability.");

        }
    }
}