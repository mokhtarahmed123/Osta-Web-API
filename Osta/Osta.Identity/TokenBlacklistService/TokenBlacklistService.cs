using Osta.Infrastructure.Caching;

namespace Osta.Identity.TokenBlacklistService
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly ICacheService cache;

        public TokenBlacklistService(ICacheService cache)
        {
            this.cache = cache;
        }

        public async Task BlacklistTokenAsync(string jti, TimeSpan expiry)
        {
            if (expiry <= TimeSpan.Zero) return;

            await cache.SetDataAsync($"blacklist:{jti}", true, expiry);
        }

        public async Task<bool> IsBlacklistedAsync(string jti)
        {
            return await cache.ExistsAsync($"blacklist:{jti}");
        }
    }
}