using FluentValidation;
using Osta.Core.Feature.Roles.Command.Model;

namespace Osta.Core.Feature.Roles.Command.Validation
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty()
                .WithMessage("Role Id is required.");

            IfRoleIsFoundThenValidateRoleName();
        }

        private void ValidateRoleName()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty()
                .WithMessage("Role name is required.")
                .MaximumLength(100)
                .WithMessage("Role name must not exceed 100 characters.");
        }

        private void IfRoleIsFoundThenValidateRoleName()
        {
            When(x => !string.IsNullOrWhiteSpace(x.RoleId), () =>
            {
                ValidateRoleName();
            });
        }
    }
}