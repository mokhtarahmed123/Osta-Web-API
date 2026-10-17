using FluentValidation;
using Osta.Core.Feature.Authorization.Query.Model.PermissionModel;

namespace Osta.Core.Feature.Authorization.Query.Validation
{
    public class RoleHasPermissionCommandValidation : AbstractValidator<RoleHasPermissionQuery>
    {
        public RoleHasPermissionCommandValidation()
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
                .WithMessage("PermissionId cannot be null.")
             ;
        }
    }
}
