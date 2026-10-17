using FluentValidation;
using Osta.Core.Feature.Authorization.Command.Model.Roles;

namespace Osta.Core.Feature.Authorization.Command.Validation.RolesValidation
{
    public class RemoveRoleFromUserCommandValidator : AbstractValidator<RemoveRoleFromUserCommand>
    {
        public RemoveRoleFromUserCommandValidator()
        {
            RuleFor(x => x.userId)
    .NotEmpty().WithMessage("UserId is required.")
    .NotNull().WithMessage("UserId cannot be null.");
            RuleFor(x => x.roleId)
                .NotEmpty().WithMessage("RoleId is required.")
                .NotNull().WithMessage("RoleId cannot be null.");

        }
    }
}
