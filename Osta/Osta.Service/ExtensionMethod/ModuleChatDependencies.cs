using Microsoft.Extensions.DependencyInjection;
using Osta.Chat.MessageService;
using Osta.Chat.Service;
using Osta.Infrastructure.ChatService;

namespace Osta.Service.ExtensionMethod;
public static class ModuleChatDependencies
{
    public static IServiceCollection AddChatDependencies(this IServiceCollection services)
    {
        services.AddSignalR();
        #region Chat

        services.AddScoped<IChatNotifier, SignalRChatNotifier>();
        services.AddScoped<IMessageService, MessageService>();



        #endregion

        return services;
    }


}

