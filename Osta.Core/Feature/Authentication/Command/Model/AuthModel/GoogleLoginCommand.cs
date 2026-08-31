using MediatR;
using Osta.Core.Bases;
using Osta.Data.Helper;

namespace Osta.Core.Feature.Authentication.Command.Model.AuthModel
{
    public record GoogleLoginCommand(string IdToken) : IRequest<Response<JWTAuthResponse>>;
    public record GoogleLoginDto
    {
        public string IdToken { get; set; }
    }
}
