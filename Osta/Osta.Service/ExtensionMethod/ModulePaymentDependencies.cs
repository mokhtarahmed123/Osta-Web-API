using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osta.Payment.CouponService;
using Osta.Payment.CouponUsageService;
using Osta.Payment.Services;
using Osta.Payment.Settings;
using Osta.Service.PaymentService;
using Stripe;

namespace Osta.Service.ExtensionMethod
{
    public static class ModulePaymentDependencies
    {
        public static IServiceCollection AddModulePaymentDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StripeSettings>(configuration.GetSection("Payment:Stripe"));


            StripeConfiguration.ApiKey = configuration["Payment:Stripe:SecretKey"];
            services.AddScoped<ICouponUsageService, CouponUsageService>();
            #region Payment

            services.AddScoped<IPaymentService, StripePaymentService>();
            services.AddScoped<IPaymentQueryService, PaymentQueryService>();
            services.AddScoped<ICouponService, Osta.Payment.CouponService.CouponService>();

            services.AddScoped<StripePaymentService>();

            #endregion


            return services;
        }
    }
}
