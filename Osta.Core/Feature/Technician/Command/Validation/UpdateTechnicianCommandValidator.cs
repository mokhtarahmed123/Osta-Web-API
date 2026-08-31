using FluentValidation;
using Microsoft.AspNetCore.Http;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;

namespace Osta.Core.Feature.Technician.Command.Validation
{
    public class UpdateTechnicianCommandValidator : AbstractValidator<UpdateTechnicianCommand>
    {
        public UpdateTechnicianCommandValidator()
        {


            RuleFor(x => x.Bio)
                .MaximumLength(500).WithMessage("Bio must not exceed 500 characters.");

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("National ID is required.")
                .Matches(@"^\d{14}$").WithMessage("National ID must be exactly 14 digits.");

            RuleFor(x => x.YearsOfExperience)
                .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative.")
                .LessThanOrEqualTo(60).WithMessage("Years of experience seems invalid.");

            RuleFor(x => x.ServiceAreas)
                .Must(areas => areas == null || areas.Count > 0)
                .WithMessage("Service areas list cannot be empty if provided.");

            RuleForEach(x => x.ServiceAreas)
                .GreaterThan(0).WithMessage("Invalid service area id.")
                .When(x => x.ServiceAreas is not null);

            RuleFor(x => x.Images!.ProfileImage)
                .Must(BeAValidImage).WithMessage("Profile image must be a valid image file (jpg, jpeg, png) under 5MB.")
                .When(x => x.Images?.ProfileImage is not null);

            RuleFor(x => x.Images!.FrontNationalIdImage)
                .Must(BeAValidImage).WithMessage("Front national ID image must be a valid image file (jpg, jpeg, png) under 5MB.")
                .When(x => x.Images?.FrontNationalIdImage is not null);

            RuleFor(x => x.Images!.BackNationalIdImage)
                .Must(BeAValidImage).WithMessage("Back national ID image must be a valid image file (jpg, jpeg, png) under 5MB.")
                .When(x => x.Images?.BackNationalIdImage is not null);
        }

        private bool BeAValidImage(IFormFile? file)
        {
            if (file is null) return true;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            const long maxFileSize = 5 * 1024 * 1024; // 5MB

            return allowedExtensions.Contains(extension) && file.Length <= maxFileSize;
        }
    }
}