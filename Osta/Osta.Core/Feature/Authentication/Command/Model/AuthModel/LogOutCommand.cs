using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authentication.Command.Model.AuthModel
{
    public record LogOutCommand : IRequest<Response<string>>;
}
