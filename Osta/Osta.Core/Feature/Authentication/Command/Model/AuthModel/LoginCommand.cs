using MediatR;
using Osta.Core.Bases;
using Osta.Data.Helper;

namespace Osta.Core.Feature.Authentication.Command.Model.AuthModel
{
    public record LoginCommand(string Email, string Password) : IRequest<Response<JWTAuthResponse>>
    {
    }
}
