using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osta.Data.Entities.Identity;
using Osta.Identity.Authentication;
using Osta.Identity.Authorization;
using Osta.Identity.DTOs;
using Osta.Identity.ExternalLogin.Facebook;
using Osta.Identity.ExternalLogin.FaceBook;
using Osta.Identity.ExternalLogin.Google;
using Osta.Identity.Models;
using Osta.Identity.Roles;
using Osta.Identity.TokenBlacklistService;
using Osta.Infrastructure.DataBase;
using System.Collections.Concurrent;

namespace Osta.Identity
{
    public static class ModuleIdentityDependencies
    {
        public static IServiceCollection AddIdentityDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddSingleton<
        ConcurrentDictionary<string, RefreshToken>>();
            services.AddScoped<IRoleService, RoleService>();

            services
                .AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<OstaContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<SignInManager<User>>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<IFacebookAuthService, FacebookAuthService>();
            services.AddScoped<ITokenBlacklistService, Osta.Identity.TokenBlacklistService.TokenBlacklistService>();


            services.Configure<JWTModel>(configuration.GetSection("JWT"));
            services.Configure<GoogleModelConfiguration>(configuration.GetSection("Authentication:Google"));
            services.Configure<FaceBookModelConfiguration>(configuration.GetSection("Facebook:Google"));


            return services;

        }
    }
}