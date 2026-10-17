using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Data.Helper;
using Osta.Identity.Authentication;

namespace Osta.Core.Feature.Authentication.Command.Handler
{
    public class RefreshTokenCommandHandler : ResponseHandler, IRequestHandler<RefreshTokenCommand, Response<JWTAuthResponse>>

    {
        private readonly IAuthenticationService authentication;

        public RefreshTokenCommandHandler(IAuthenticationService authentication)
        {
            this.authentication = authentication;
        }
        public async Task<Response<JWTAuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var Result = await authentication.GetRefreshToken(request.RefreshToken, request.Token);
            return Success<JWTAuthResponse>(Result);
        }
    }
}
