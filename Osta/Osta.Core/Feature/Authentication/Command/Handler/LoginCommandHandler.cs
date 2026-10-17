using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Bases;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Data.Entities.Identity;
using Osta.Data.Helper;
using Osta.Identity.Authentication;

namespace Osta.Core.Feature.Authentication.Command.Handler
{
    public class LoginCommandHandler : ResponseHandler, IRequestHandler<LoginCommand, Response<JWTAuthResponse>>
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IAuthenticationService authentication;

        public LoginCommandHandler(UserManager<User> userManager, SignInManager<User> signInManager, IAuthenticationService authentication)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.authentication = authentication;
        }
        public async Task<Response<JWTAuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null) return NotFound<JWTAuthResponse>(" User With Email " + request.Email + " Not Found ");

            var Password = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!user.EmailConfirmed)
                return BadRequest<JWTAuthResponse>();

            if (!Password.Succeeded) return BadRequest<JWTAuthResponse>("Password is incorrect.");
            var Result = await authentication.GenerateJWToken(user);
            return Success<JWTAuthResponse>(Result);
        }
    }
}
