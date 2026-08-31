using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osta.BackgroundJobs.AppointmentJob;

namespace Osta.BackgroundJobs
{
    public static class ModuleBackgroundJobsDependencies
    {
        public static IServiceCollection AddBackgroundJobsDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config
            .SetDataCompatibilityLevel(
             CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
             .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(
    configuration.GetConnectionString(
        "osta")));

            services.AddHangfireServer();

            // Jobs
            services.AddScoped<IAppointmentReminderJob, AppointmentReminderJob>();
            return services;
        }


    }
}
