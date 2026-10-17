using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Osta.Core.Bases;
using Osta.Core.Behavior;
using System.Reflection;

namespace Osta.Core
{
    public static class ModuleCoreDependencies
    {
        public static IServiceCollection AddModuleCoreDependencies(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(ModuleCoreDependencies).Assembly));

            services.AddAutoMapper(cfg =>
                cfg.AddMaps(typeof(ModuleCoreDependencies).Assembly));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddScoped<IResponseHandler, ResponseHandler>();
            services.AddScoped<IPaymentQueryService, PaymentQueryService>();
            return services;


        }

    }
}
