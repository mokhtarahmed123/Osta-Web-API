namespace Osta.Identity.DTOs
{
    public class RefreshTokenResponse
    {
        public string RefreshToken { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
    }
}
