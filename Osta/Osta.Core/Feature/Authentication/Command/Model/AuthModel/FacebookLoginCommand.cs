using MediatR;
using Osta.Core.Bases;
using Osta.Data.Helper;

namespace Osta.Core.Feature.Authentication.Command.Model.AuthModel
{
    public record FacebookLoginCommand : IRequest<Response<JWTAuthResponse>>
    {
        public string AccessToken { get; set; }
    }
}
