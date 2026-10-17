using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authorization.Command.Model.Roles
{
    public record RemoveRoleFromUserCommand(string roleId, string userId) : IRequest<Response<string>>
 ;
}
