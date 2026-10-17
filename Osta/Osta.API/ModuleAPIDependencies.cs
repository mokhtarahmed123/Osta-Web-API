using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Osta.API.Jobs;
using Osta.Infrastructure.DataBase;

namespace Osta.API
{
    public static class ModuleAPIDependencies
    {
        public static IServiceCollection AddModuleApiDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            #region Database
            services.AddDbContext<OstaContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("osta")));
            #endregion

            #region Redis
            // Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "Osta:";
            });
            #endregion

            #region ApiVersioning

            services.AddApiVersioning(
                opt =>
                {
                    opt.AssumeDefaultVersionWhenUnspecified = true;
                    opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                    opt.ReportApiVersions = true;
                    opt.ApiVersionReader = ApiVersionReader.Combine(
                        new HeaderApiVersionReader("x-version")
                            );
                }
                );

            services.AddVersionedApiExplorer(
                opt =>
                {
                    opt.GroupNameFormat = "'v'VVV";
                    opt.SubstituteApiVersionInUrl = true;

                }
                );
            #endregion

            #region CORS
            services.AddCors(options =>
                     options.AddPolicy("CorsPolicy", policyBuilder =>
                         policyBuilder.SetIsOriginAllowed(_ => true)

                                      .AllowAnyMethod()
                                      .AllowAnyHeader().AllowCredentials())

                 );
            #endregion
            services.AddScoped<AppointmentReminderJob>();


            return services;


        }


    }
}
