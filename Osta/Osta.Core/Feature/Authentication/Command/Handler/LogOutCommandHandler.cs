using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Bases;
using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Data.Entities.Identity;
using Osta.Identity.Authentication;
using Osta.Identity.TokenBlacklistService;
using Osta.Infrastructure.Caching;
using Osta.SharedKernel.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Osta.Core.Feature.Authentication.Command.Handler
{
    public class LogOutCommandHandler : ResponseHandler, IRequestHandler<LogOutCommand, Response<string>>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAuthenticationService authentication;
        private readonly SignInManager<User> signInManager;
        private readonly ITokenBlacklistService tokenBlacklistService;
        private readonly ICacheService cacheService;

        public LogOutCommandHandler(ICurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor,
            IAuthenticationService authentication, SignInManager<User> signInManager,
            ITokenBlacklistService tokenBlacklistService, ICacheService cacheService)
        {
            this.currentUserService = currentUserService;
            this.httpContextAccessor = httpContextAccessor;
            this.authentication = authentication;
            this.signInManager = signInManager;
            this.tokenBlacklistService = tokenBlacklistService;
            this.cacheService = cacheService;
        }
        public async Task<Response<string>> Handle(LogOutCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return NotFound<string>("User not found");

            var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                .ToString();

            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token["Bearer ".Length..].Trim();

                try
                {
                    var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                    var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                    var expiry = jwtToken.ValidTo - DateTime.UtcNow;

                    if (!string.IsNullOrEmpty(jti) && expiry > TimeSpan.Zero)
                        await cacheService.SetDataAsync($"blacklist:{jti}", true, expiry);
                }
                catch (Exception)
                {

                }
            }

            await authentication.RevokeRefreshToken(userId);
            await signInManager.SignOutAsync();
            return Success("Logged out successfully");
        }
    }
}
