using FluentValidation;
using Osta.Core.Feature.Roles.Command.Model;
using Osta.Identity.Roles;

namespace Osta.Core.Feature.Roles.Command.Validation
{
    public class AddRoleCommandValidator : AbstractValidator<AddRoleCommand>
    {
        private readonly IRoleService roleService;

        public AddRoleCommandValidator(IRoleService roleService)
        {
            this.roleService = roleService;
            ValidateRoleName();
            ValidateRoleUniqueness();
        }
        private void ValidateRoleName()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.");
        }
        private void ValidateRoleUniqueness()
        {

            RuleFor(x => x.Name)
                .MustAsync(async (name, cancellationToken) =>
                    !await roleService.RoleExistsAsync(name))
                .WithMessage("Role name must be unique.");
        }
    }
}
