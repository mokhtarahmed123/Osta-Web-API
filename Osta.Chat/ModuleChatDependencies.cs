using Microsoft.Extensions.DependencyInjection;
using Osta.Chat.MessageService;
using Osta.Chat.Service;

namespace Osta.Chat
{
    public static class ModuleChatDependencies
    {
        public static IServiceCollection AddChatDependencies(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddScoped<IChatNotifier, SignalRChatNotifier>();
            services.AddScoped<IMessageService, Osta.Chat.Service.MessageService>();
            return services;
        }


    }
}
