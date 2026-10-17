using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.ServiceArea.Command.Model
{
    public record DeleteServiceAreaCommand(int id) : IRequest<Response<string>>;
}
