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
<<<<<<< HEAD
        .UseSqlServerStorage(
    configuration.GetConnectionString(
=======
        .UseSqlServerStorage(configuration.GetConnectionString(
>>>>>>> c73a0b6 (Initial commit)
        "osta")));

            services.AddHangfireServer();

            // Jobs
            services.AddScoped<IAppointmentReminderJob, AppointmentReminderJob>();
            return services;
        }


    }
}
