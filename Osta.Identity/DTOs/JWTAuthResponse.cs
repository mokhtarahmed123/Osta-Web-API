using Osta.Identity.DTOs;

namespace Osta.Data.Helper
{
    public class JWTAuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public RefreshTokenResponse RefreshToken { get; set; } = null!;
    }
}
