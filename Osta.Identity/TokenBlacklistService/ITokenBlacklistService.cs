namespace Osta.Identity.TokenBlacklistService
{
    public interface ITokenBlacklistService
    {
        Task BlacklistTokenAsync(string jti, TimeSpan expiry);
        Task<bool> IsBlacklistedAsync(string jti);
    }
}
