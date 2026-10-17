using AutoMapper;
using MediatR;
using Osta.Chat.MessageService;
using Osta.Chat.Model;
using Osta.Core.Bases;
using Osta.Core.Feature.Chat.Query.Model;

namespace Osta.Core.Feature.Chat.Query.Handler
{
    public class GetBookingMessagesHandler : ResponseHandler,
        IRequestHandler<GetBookingMessagesQuery, Response<List<MessageModel>>>
    {
        private readonly IMapper mapper;
        private readonly IMessageService messageService;

        public GetBookingMessagesHandler(IMapper mapper, IMessageService messageService)
        {
            this.mapper = mapper;
            this.messageService = messageService;
        }
        public async Task<Response<List<MessageModel>>> Handle(GetBookingMessagesQuery request, CancellationToken cancellationToken)
        {
            var List = await messageService.GetMessageByBookingId(request.BookingId);
            return Success(mapper.Map<List<MessageModel>>(List));

        }
    }
}
