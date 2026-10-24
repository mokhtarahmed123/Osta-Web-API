using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osta.BackgroundJobs.AppointmentJob;
using Osta.Infrastructure.AppointmentJobService;

namespace Osta.Service.ExtensionMethod;

public static class ModuleBackgroundJobsDependencies
{
    public static IServiceCollection AddBackgroundJobsDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
        .SetDataCompatibilityLevel(
         CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
         .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(configuration.GetConnectionString(
    "osta")));

        services.AddHangfireServer();

        #region Background Job


        services.AddScoped<IAppointmentReminderJob, AppointmentReminderJob>();

        #endregion



        // Jobs
        return services;
    }


}

