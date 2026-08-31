
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osta.Data.Helper;
using Osta.Notification.Email;
using Osta.Notification.Interfaces;
using Osta.Notification.Queue;

namespace Osta
{
    public static class ModuleNotificationDependencies
    {
        public static IServiceCollection AddModuleNotificationDependencies(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISendNotificationMessage, SendNotificationMessage>();
            services.Configure<EmailSettings>(Configuration.GetSection("Email"));

            return services;

        }
    }
}