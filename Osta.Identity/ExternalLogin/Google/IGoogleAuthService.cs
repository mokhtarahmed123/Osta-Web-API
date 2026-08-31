using Osta.Identity.Models;

namespace Osta.Identity.ExternalLogin.Google
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserModel> ValidateIdTokenAsync(string idToken);
    }
}
