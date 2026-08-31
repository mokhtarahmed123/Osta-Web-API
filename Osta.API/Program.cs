using Hangfire;
using Osta.BackgroundJobs;
using Osta.Booking;
using Osta.Chat;
using Osta.Chat.Hub;
using Osta.Core;
using Osta.Identity;
using Osta.Infrastructure;
using Osta.Payment;
using Osta.Service;
namespace Osta.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();

            #region CORS
            builder.Services.AddCors(options =>
            options.AddPolicy("CorsPolicy", policyBuilder =>
                policyBuilder.AllowAnyOrigin()
                             .AllowAnyMethod()
                             .AllowAnyHeader())
        );
            #endregion

            builder.Services.AddInfrastructureDependencies(builder.Configuration).
                AddServiceDependencies().AddBookingDependencies().
                AddBackgroundJobsDependencies(builder.Configuration).
               AddModuleSharedKernelDependencies().AddIdentityDependencies(builder.Configuration).
               AddModuleNotificationDependencies(builder.Configuration)
                .AddModuleCoreDependencies().AddChatDependencies().
                AddModuleApiDependencies(builder.Configuration).
                AddModulePaymentDependencies(builder.Configuration);


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI();
            }



            app.UseHangfireDashboard("/hangfire");

            RecurringJob.AddOrUpdate<Osta.API.Jobs.AppointmentReminderJob>(
                "appointment-reminder-job",
                job => job.RunAsync(),
              "*/20 * * * *"
            );

            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseMiddleware<BlacklistCheckMiddleware>();

            app.UseAuthorization();
            app.MapHub<ChatHub>("/hubs/chat");
            app.MapControllers();

            app.Run();
        }
    }
}
