using FluentValidation;
using Osta.Core.Feature.Authorization.Command.Model.PermissionModel;

namespace Osta.Core.Feature.Authorization.Command.Validation.PermissionValidation
{
    public class AssignPermissionToRoleCommandValidation : AbstractValidator<AssignPermissionToRoleCommand>
    {
        public AssignPermissionToRoleCommandValidation()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is required.")
                .NotNull().WithMessage("RoleId cannot be null.");
            RuleFor(x => x.PermissionIds)
                .NotEmpty().WithMessage("PermissionIds is required.")
                .NotNull().WithMessage("PermissionIds cannot be null.");
        }
    }
}
