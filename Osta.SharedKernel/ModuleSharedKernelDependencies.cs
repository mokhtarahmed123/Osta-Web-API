using Microsoft.Extensions.DependencyInjection;
using Osta.SharedKernel;
using Osta.SharedKernel.Identity;

namespace Osta
{
    public static class ModuleSharedKernelDependencies
    {
        public static IServiceCollection AddModuleSharedKernelDependencies(this IServiceCollection services)
        {
            // Add shared kernel dependencies here
            // Example: services.AddSingleton<IMyService, MyService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IFileService, FileService>();
            return services;
        }

    }
}
