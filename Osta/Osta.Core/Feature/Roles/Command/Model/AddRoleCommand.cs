using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Roles.Command.Model
{
    public record AddRoleCommand : IRequest<Response<string>>
    {
        public string Name { get; init; }

    }
}
