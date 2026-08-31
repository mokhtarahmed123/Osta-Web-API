using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Roles.Command.Model
{
    public record DeleteRoleCommand(string RoleId) : IRequest<Response<string>>;
}
