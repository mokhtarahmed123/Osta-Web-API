using Microsoft.Extensions.DependencyInjection;
using Osta.SharedKernel;
using Osta.SharedKernel.Identity;

namespace Osta
{
    public static class ModuleSharedKernelDependencies
    {
        public static IServiceCollection AddModuleSharedKernelDependencies(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IFileService, FileService>();
            return services;
        }

    }
}
