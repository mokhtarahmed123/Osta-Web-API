using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osta.Payment.CouponService;
using Osta.Payment.Services;
using Osta.Payment.Settings;
using Stripe;

namespace Osta.Payment
{
    public static class ModulePaymentDependencies
    {
        public static IServiceCollection AddModulePaymentDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StripeSettings>(configuration.GetSection("Payment:Stripe"));


            StripeConfiguration.ApiKey = configuration["Payment:Stripe:SecretKey"];
            services.AddScoped<IPaymentService, StripePaymentService>();
            services.AddScoped<ICouponService, Osta.Payment.CouponService.CouponService>();


            services.AddScoped<StripePaymentService>();


            return services;
        }
    }
}
