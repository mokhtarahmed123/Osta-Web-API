using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Osta.Identity.Models;

namespace Osta.Identity.ExternalLogin.Google
{
    public class GoogleAuthService : IGoogleAuthService
    {

        private readonly GoogleModelConfiguration _googleModelConfiguration;

        public GoogleAuthService(IOptions<GoogleModelConfiguration> GoogleModelConfiguration)
        {

            _googleModelConfiguration = GoogleModelConfiguration.Value;
        }
        public async Task<GoogleUserModel> ValidateIdTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleModelConfiguration.ClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                return new GoogleUserModel
                {
                    Id = payload.Subject,
                    Email = payload.Email,
                    Name = payload.Name,
                    PictureUrl = payload.Picture,
                    EmailVerified = payload.EmailVerified
                };
            }
            catch (InvalidJwtException)
            {
                throw new UnauthorizedAccessException("Invalid Google token");
            }
        }
    }
}
