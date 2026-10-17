using AutoMapper;
using MediatR;
using Osta.Booking.Interface;
using Osta.Chat.MessageService;
using Osta.Chat.Model;
using Osta.Chat.Service;
using Osta.Core.Bases;
using Osta.Core.Feature.Chat.Command.Model;
using Osta.Core.HandlerMiddleware;
using Osta.Domain.Entities.Chat;

namespace Osta.Core.Feature.Chat.Command.Handler
{
    public class SendMessageHandler : ResponseHandler, IRequestHandler<SendMessageCommand, Response<MessageModel>>
    {

        private readonly IMapper _mapper;
        private readonly IChatNotifier _notifier;
        private readonly IBookingService bookingService;
        private readonly IMessageService messageService;

        public SendMessageHandler(IMapper mapper, IChatNotifier notifier, IBookingService bookingService, IMessageService messageService)
        {
            _mapper = mapper;
            _notifier = notifier;
            this.bookingService = bookingService;
            this.messageService = messageService;
        }

        public async Task<Response<MessageModel>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var booking = await bookingService.GetBookingById(request.BookingId) ?? throw new NotFoundException("Booking not found");
            if (request.SenderId != booking.CustomerId && request.SenderId != booking.TechnicianId)
                throw new ForbiddenException("Not a participant in this booking");

            var Message = _mapper.Map<Message>(request);
            await messageService.SendMessage(Message);

            var dto = _mapper.Map<MessageModel>(Message);

            await _notifier.NotifyNewMessage(request.BookingId, dto);

            return Success(dto);
        }
    }
}
