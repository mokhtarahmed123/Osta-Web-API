using FluentValidation;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Technician.Command.Validation
{
    public class RequestTechnicianCommandValidation : AbstractValidator<AddTechnicianCommand>
    {
        private readonly ITechnicianService technicianService;
        private readonly ICurrentUserService currentUser;

        public RequestTechnicianCommandValidation(ITechnicianService technicianService, ICurrentUserService currentUser)
        {
            this.technicianService = technicianService;
            this.currentUser = currentUser;
            ValidateRules();
            ValidateUserExists();
        }

        private void ValidateRules()
        {
            RuleFor(x => x.Bio)
                .MaximumLength(500)
                .WithMessage("Bio must not exceed 500 characters.");

            RuleFor(x => x.YearsOfExperience)
                .GreaterThanOrEqualTo(0)
                .WithMessage("YearsOfExperience cannot be negative.")
                .LessThanOrEqualTo(60)
                .WithMessage("YearsOfExperience must not exceed 60 years.");

            RuleFor(x => x.NationalId)
                .NotEmpty()
                .WithMessage("National ID is required.")
                .Length(14)
                .WithMessage("National ID must be exactly 14 characters long.");


            RuleFor(x => x.Images.ProfileImage).NotNull().WithMessage("Profile image is required.");
            RuleFor(x => x.Images.FrontNationalIdImage).NotNull().WithMessage("Front national ID image is required.");
            RuleFor(x => x.Images.BackNationalIdImage).NotNull().WithMessage("Back national ID image is required.");
        }

        private void ValidateUserExists()
        {
            var id = currentUser.UserId;

            RuleFor(x => x)
                .NotEmpty()
                .WithMessage("Id is required.")
                .MustAsync(async (_, cancellation) =>
                   !await technicianService.TechnicianExistsAsync(id))
                .WithMessage("  This Technician Is Already Found.");
        }

    }
}