
using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Bases;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Data.Entities.Identity;
using Osta.Data.Helper;
using Osta.Identity.Authentication;
using Osta.Identity.ExternalLogin.FaceBook;

namespace Osta.Core.Feature.Authentication.Command.Handler
{
    public class FacebookLoginCommandHandler : ResponseHandler, IRequestHandler<FacebookLoginCommand, Response<JWTAuthResponse>>
    {
        private readonly UserManager<User> userManager;
        private readonly IAuthenticationService authentication;
        private readonly IFacebookAuthService facebookAuthService;

        public FacebookLoginCommandHandler(UserManager<User> userManager, IAuthenticationService authentication,
            IFacebookAuthService facebookAuthService)
        {
            this.userManager = userManager;
            this.authentication = authentication;
            this.facebookAuthService = facebookAuthService;
        }

        public async Task<Response<JWTAuthResponse>> Handle(FacebookLoginCommand request, CancellationToken cancellationToken)
        {
            var fbUser = await facebookAuthService.ValidateAccessTokenAsync(request.AccessToken);

            if (string.IsNullOrEmpty(fbUser.Email))
                throw new UnauthorizedAccessException("Facebook account has no email permission granted");

            var user = await userManager.FindByEmailAsync(fbUser.Email);

            if (user == null)
            {
                user = new User
                {
                    UserName = fbUser.Email,
                    Email = fbUser.Email,
                    FullName = fbUser.Name,
                    EmailConfirmed = true,
                    ProfileImage = fbUser.PictureUrl,
                    IsActive = true,
                    Provider = "Facebook",
                    ExternalId = fbUser.Id
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
                user.ExternalId = fbUser.Id;
                user.Provider = "Facebook";
                await userManager.UpdateAsync(user);
            }

            var jwtResponse = await authentication.GenerateJWToken(user);
            return Success(jwtResponse);
        }
    }
}