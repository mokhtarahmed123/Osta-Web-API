using Osta.Domain.Entities.Chat;

namespace Osta.Chat.MessageService
{
    public interface IMessageService
    {
        Task SendMessage(Message message, CancellationToken ct = default);
        Task<IEnumerable<Message>> GetMessageByBookingId(int BookingId, CancellationToken ct = default);

    }
}
