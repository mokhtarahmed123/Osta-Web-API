using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Service.Command.Model
{
    public record DeleteServiceCommand(int Id) : IRequest<Response<string>>;

}
