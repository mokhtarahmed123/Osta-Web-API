using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authorization.Query.Model.PermissionModel
{
    public record RoleHasPermissionQuery(string RoleId, string PermissionId) : IRequest<Response<bool>>;

}
