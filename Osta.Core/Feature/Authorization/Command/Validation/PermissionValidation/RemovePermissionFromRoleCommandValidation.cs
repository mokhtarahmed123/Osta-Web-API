using FluentValidation;
using Osta.Core.Feature.Authorization.Command.Model.PermissionModel;

namespace Osta.Core.Feature.Authorization.Command.Validation.PermissionValidation
{
    public class RemovePermissionFromRoleCommandValidation : AbstractValidator<RemovePermissionFromRoleCommand>
    {
        public RemovePermissionFromRoleCommandValidation()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty()
                .WithMessage("RoleId is required.")
                .NotNull()
                .WithMessage("RoleId cannot be null.");

            RuleFor(x => x.PermissionId)
                .NotEmpty()
                .WithMessage("PermissionId is required.")
                .NotNull()
                .WithMessage("PermissionId cannot be null.");
        }


    }
}
