using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authorization.Command.Model.PermissionModel
{
    public record RemovePermissionFromRoleCommand(string RoleId, string PermissionId) : IRequest<Response<string>>
   ;
}
