using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authorization.Query.Model.PermissionModel
{
    public record GetRolePermissionsQuery(string roleId) : IRequest<Response<IList<string>>>;


}
