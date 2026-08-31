using Osta.Data.Helper;
using Osta.Notification.Email;
using Osta.Notification.Interfaces;
using Osta.Notification.Worker.Consumers;

namespace Osta.Notification.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("Email"));
            builder.Services.AddHostedService<NotificationConsumer>();
            builder.Services.AddHostedService<PayoutNotificationConsumer>();
            builder.Services.AddHostedService<AppointmentNotificationConsumer>();
            builder.Services.AddHostedService<BookingNotificationConsumer>();
            builder.Services.AddHostedService<TechnicianStatusNotificationConsumer>();
            var host = builder.Build();
            host.Run();
        }
    }
}