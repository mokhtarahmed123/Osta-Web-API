using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authorization.Query.Model
{
    public record UserIsInRoleQuery(string UserId, string RoleId) : IRequest<Response<bool>>
   ;
}
