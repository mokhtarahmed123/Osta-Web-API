using Osta.Identity.Models;

namespace Osta.Identity.ExternalLogin.FaceBook
{
    public interface IFacebookAuthService
    {
        Task<FacebookUserModel> ValidateAccessTokenAsync(string accessToken);
    }
}
