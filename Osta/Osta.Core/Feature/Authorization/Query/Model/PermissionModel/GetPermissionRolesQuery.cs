using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authorization.Query.Model.PermissionModel
{
    public record GetPermissionRolesQuery(string PermissionId) : IRequest<Response<IList<string>>>
   ;
}
