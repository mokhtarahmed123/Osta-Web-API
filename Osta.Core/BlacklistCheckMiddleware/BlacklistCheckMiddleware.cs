using Microsoft.AspNetCore.Http;
using Osta.Infrastructure.Caching;
using System.IdentityModel.Tokens.Jwt;

public class BlacklistCheckMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICacheService cacheService)
    {
        var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        if (!string.IsNullOrEmpty(jti))
        {
            var isBlacklisted = await cacheService.ExistsAsync($"blacklist:{jti}");
            if (isBlacklisted)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token has been revoked.");
                return;
            }
        }

        await next(context);
    }
}