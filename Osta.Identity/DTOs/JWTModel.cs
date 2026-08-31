namespace Osta.Identity.DTOs
{
    public class JWTModel
    {
        public string AudienceIP { get; set; } = string.Empty;
        public string IssuerIP { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int AccessTokenExpiredDate { get; set; }
        public int RefreshTokenExpiredDate { get; set; }

    }
}
