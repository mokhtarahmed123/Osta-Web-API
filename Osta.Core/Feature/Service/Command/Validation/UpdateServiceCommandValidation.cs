using FluentValidation;
using Microsoft.AspNetCore.Http;
using Osta.Core.Feature.Service.Command.Model;
using Osta.Service.Abstract.ServicesAbstract;

namespace Osta.Core.Feature.Service.Command.Validation
{
    public class UpdateServiceCommandValidation : AbstractValidator<UpdateServiceCommand>
    {
        private readonly ICategoryService categoryService;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public UpdateServiceCommandValidation(ICategoryService categoryService)
        {
            this.categoryService = categoryService;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than zero.")
                .MustAsync(async (categoryId, cancellation) =>
                {
                    var category = await categoryService.GetCategoryAsync(categoryId);
                    return category != null;
                })
                .WithMessage("CategoryId does not exist.");

            RuleFor(x => x.Image)
                .Must(BeAValidImageExtension)
                .When(x => x.Image != null)
                .WithMessage("Invalid image format. Allowed formats are: .jpg, .jpeg, .png, .gif , .webp");
        }

        private bool BeAValidImageExtension(IFormFile? image)
        {
            if (image == null) return true;

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }
    }
}