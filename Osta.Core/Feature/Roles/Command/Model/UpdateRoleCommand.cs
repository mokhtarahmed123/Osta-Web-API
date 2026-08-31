using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Roles.Command.Model
{
    public record UpdateRoleCommand(string RoleId) : IRequest<Response<string>>
    {
        public string RoleName { get; init; }

    }
}
