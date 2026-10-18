using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osta.Data.Entities.Identity;
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

namespace Osta.Service.ExtensionMethod
{
    public static class ModuleIdentityDependencies
    {
        public static IServiceCollection AddIdentityDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<JWTModel>(configuration.GetSection("JWT"));
            services.Configure<GoogleModelConfiguration>(configuration.GetSection("Authentication:Google"));
            services.Configure<FaceBookModelConfiguration>(configuration.GetSection("Facebook:Google"));

            #region Identity Service
            services.AddScoped<Identity.Authentication.IAuthenticationService, IdentityService.AuthenticationService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();


            services.AddSingleton<
            ConcurrentDictionary<string, RefreshToken>>();
            services.AddScoped<IRoleService, RoleService>();

            services
                .AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<OstaContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<IFacebookAuthService, FacebookAuthService>();

            services.AddScoped<SignInManager<User>>();
            services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();

            #endregion



            return services;

        }
    }
}