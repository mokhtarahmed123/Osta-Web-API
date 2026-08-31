using MediatR;
using Osta.Core.Bases;
using Osta.Data.Helper;

namespace Osta.Core.Feature.Authentication.Command.Model.AuthModel
{
    public record RefreshTokenCommand(string RefreshToken, string Token) : IRequest<Response<JWTAuthResponse>>;


}
