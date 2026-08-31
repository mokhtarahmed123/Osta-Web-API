using MediatR;
using Osta.Chat.Model;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Chat.Query.Model
{
    public record GetBookingMessagesQuery(int BookingId) : IRequest<Response<List<MessageModel>>>;

}
