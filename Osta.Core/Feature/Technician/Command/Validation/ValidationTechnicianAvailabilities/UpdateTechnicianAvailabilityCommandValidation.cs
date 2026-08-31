using FluentValidation;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianAvailabilities;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Technician.Command.Validation.ValidationTechnicianAvailabilities
{
    public class UpdateTechnicianAvailabilityCommandValidation
        : AbstractValidator<UpdateTechnicianAvailabilityCommand>
    {
        private readonly ICurrentUserService currentUser;

        public UpdateTechnicianAvailabilityCommandValidation(
            ITechnicianAvailabilityService technicianAvailabilityService, ICurrentUserService currentUser)
        {
            this.currentUser = currentUser;
            RuleFor(x => x.DayOfWeek)
                .IsInEnum()
                .WithMessage("Invalid day of week.");

            RuleFor(x => x)
                .Must(x => x.EndTime > x.StartTime)
                .WithMessage("End time must be greater than start time.");

            RuleFor(x => x)
                .MustAsync(async (model, cancellation) =>
                {
                    var availability = new TechnicianAvailability
                    {
                        Id = model.Id,
                        TechnicianId = currentUser.UserId,
                        DayOfWeek = model.DayOfWeek,
                        StartTime = model.StartTime,
                        EndTime = model.EndTime,
                        IsAvailable = model.IsAvailable
                    };

                    return !await technicianAvailabilityService.HasOverlappingAvailabilityForUpdateAsync(availability);
                })
                .WithMessage("This availability already exists.");

        }
    }
}