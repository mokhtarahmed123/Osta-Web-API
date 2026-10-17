using Microsoft.AspNetCore.SignalR;
using Osta.Chat.Hub;
using Osta.Chat.Model;

namespace Osta.Chat.Service
{
    public class SignalRChatNotifier : IChatNotifier
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRChatNotifier(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNewMessage(int bookingId, MessageModel message)
        {
            await _hubContext.Clients.Group($"booking-{bookingId}")
                .SendAsync("ReceiveMessage", message);
        }
    }
}
