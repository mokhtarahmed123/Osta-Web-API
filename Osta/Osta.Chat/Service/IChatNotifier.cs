using Osta.Chat.Model;

namespace Osta.Chat.Service
{
    public interface IChatNotifier
    {
        Task NotifyNewMessage(int bookingId, MessageModel message);

    }
}
