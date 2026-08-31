using FluentValidation;
using Osta.Core.Feature.Category.Command.Model;
using Osta.Service.Abstract.ServicesAbstract;

namespace Osta.Core.Feature.Category.Command.Validation
{
    public class AddCategoryCommandValidator : AbstractValidator<AddCategoryCommand>
    {
        private readonly ICategoryService categoryService;

        public AddCategoryCommandValidator(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
            validateAddCategoryCommand();
            IfNameIsFound();
        }
        public void validateAddCategoryCommand()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Image)
                .Must(f => f == null || f.Length <= 5 * 1024 * 1024)
                .WithMessage("Image size must not exceed 5MB.");
        }
        public void IfNameIsFound()
        {
            RuleFor(x => x.Name)
                .MustAsync(async (name, cancellation) =>
                {

                    bool nameExists = await categoryService.IsCategoryNameExistsAsync(name, cancellation);
                    return !nameExists;
                })
                .WithMessage("A category with the same name already exists.");
        }
    }
}
