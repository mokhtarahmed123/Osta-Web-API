using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Bases;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Data.Entities.Identity;
using Osta.Data.Helper;
using Osta.Identity.Authentication;
using Osta.Identity.ExternalLogin.Google;

namespace Osta.Core.Feature.Authentication.Command.Handler
{
    public class GoogleLoginCommandHandler : ResponseHandler, IRequestHandler<GoogleLoginCommand, Response<JWTAuthResponse>>
    {
        private readonly UserManager<User> userManager;

        private readonly IAuthenticationService authentication;

        private readonly IGoogleAuthService googleAuthService;

        public GoogleLoginCommandHandler(UserManager<User> userManager, IAuthenticationService authentication,
     IGoogleAuthService googleAuthService)
        {
            this.userManager = userManager;

            this.authentication = authentication;

            this.googleAuthService = googleAuthService;
        }

        public async Task<Response<JWTAuthResponse>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {

            var googleUser = await googleAuthService.ValidateIdTokenAsync(request.IdToken);

            if (!googleUser.EmailVerified)
                throw new UnauthorizedAccessException("Google email not verified");


            var user = await userManager.FindByEmailAsync(googleUser.Email);

            if (user == null)
            {

                user = new User
                {
                    UserName = googleUser.Email,
                    Email = googleUser.Email,
                    FullName = googleUser.Name,
                    EmailConfirmed = true,
                    ProfileImage = googleUser.PictureUrl,
                    Provider = "Google",
                    ExternalId = googleUser.Id,
                    IsActive = true
                };

                var createResult = await userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create user: {errors}");
                }

                await userManager.AddToRoleAsync(user, "User");
            }
            else if (string.IsNullOrEmpty(user.ExternalId))
            {

                user.ExternalId = googleUser.Id;
                user.Provider = "Google";
                user.IsActive = true;
                await userManager.UpdateAsync(user);
            }

            var jwtResponse = await authentication.GenerateJWToken(user);

            return Success(jwtResponse);
        }
    }
}