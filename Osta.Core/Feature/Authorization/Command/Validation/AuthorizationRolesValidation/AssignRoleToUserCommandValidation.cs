using FluentValidation;
using Osta.Core.Feature.Authorization.Command.Model.Roles;

namespace Osta.Core.Feature.Authorization.Command.Validation.RolesValidation
{
    public class AssignRoleToUserCommandValidation : AbstractValidator<AssignRoleToUserCommand>
    {
        public AssignRoleToUserCommandValidation()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .NotNull().WithMessage("UserId cannot be null.");
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is required.")
                .NotNull().WithMessage("RoleId cannot be null.");
        }
    }
}
