using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authorization.Command.Model.PermissionModel
{
    public record AssignPermissionToRoleCommand(List<string> PermissionIds, string RoleId) : IRequest<Response<string>>
    ;

}
