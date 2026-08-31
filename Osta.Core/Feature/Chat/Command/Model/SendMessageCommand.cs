using MediatR;
using Osta.Chat.Model;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Chat.Command.Model
{
    public record SendMessageCommand(int BookingId, string SenderId, string Content) : IRequest<Response<MessageModel>>;

}
