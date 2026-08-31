using Microsoft.AspNetCore.Http;
using Osta.Core.HandlerMiddleware;
using System.Security.Claims;

namespace Osta.SharedKernel.Identity
{
    class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        #region Properties

        private ClaimsPrincipal? User =>
            httpContextAccessor.HttpContext?.User;
        public string UserId =>
            GetClaim("Id")
            ?? throw new NotFoundException("User Id claim not found");
        public string UserIdFromJWT => UserIdFromJWTFunction() ?? throw new NotFoundException("User Id claim not found");
        public string? UserIdFromJWTWithNull => UserIdFromJWTWithNullFunction();
        public string? Email => GetClaim(ClaimTypes.Email);

        public IEnumerable<string> Roles => User?.
            FindAll(ClaimTypes.Role).Select(r => r.Value)
            ?? Enumerable.Empty<string>();

        #endregion

        #region Methods
        private string? GetClaim(string claimType)
        {
            var user = httpContextAccessor.HttpContext?.User;


            return user?.FindFirst(claimType)?.Value;
        }

        private string UserIdFromJWTFunction()
        {
            var user = httpContextAccessor.HttpContext?.User;
            var claim = user?.FindFirst("Id")?.Value;

            if (string.IsNullOrEmpty(claim))
                throw new Exception("User Id claim not found");

            return claim;

        }

        private string? UserIdFromJWTWithNullFunction()
        {
            var user = httpContextAccessor.HttpContext?.User;
            var claim = user?.FindFirst("Id")?.Value;

            return claim;
        }
        #endregion

    }
}
