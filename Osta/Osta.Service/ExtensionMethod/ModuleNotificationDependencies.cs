using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osta.Data.Helper;
using Osta.Notification.Interfaces;
using Osta.Notification.Queue;
using Osta.Service.NotificationService.Email;
using Osta.Service.NotificationService.Queue;

namespace Osta.Service.ExtensionMethod
{
    public static class ModuleNotificationDependencies
    {
        public static IServiceCollection AddModuleNotificationDependencies(this IServiceCollection services, IConfiguration Configuration)
        {

            services.Configure<EmailSettings>(Configuration.GetSection("Email"));
            #region Notification
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISendNotificationMessage, SendNotificationMessage>();
            #endregion
            return services;

        }
    }
}