using FluentValidation;
using Osta.Core.Feature.Category.Command.Model;
using Osta.Service.Abstract.ServicesAbstract;

namespace Osta.Core.Feature.Category.Command.Validation
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        private readonly ICategoryService categoryService;

        public UpdateCategoryCommandValidator(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
            Validate();
            NameIsExists();
        }
        private void Validate()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Image)
                .Must(f => f == null || f.Length <= 5 * 1024 * 1024)
                .WithMessage("Image size must not exceed 5MB.");

        }
        private void NameIsExists()
        {
            RuleFor(x => x)
                .MustAsync(async (model, cancellation) =>
                    !await categoryService.IsCategoryNameExistsForOtherCategoryAsync(model.Name, model.Id, cancellation))
                .WithMessage("Name already exists.");
        }
    }
}
