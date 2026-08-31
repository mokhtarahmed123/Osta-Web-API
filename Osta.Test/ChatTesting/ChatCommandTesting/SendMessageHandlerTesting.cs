using AutoMapper;
using Moq;
using Osta.Booking.Interface;
using Osta.Chat.MessageService;
using Osta.Chat.Model;
using Osta.Chat.Service;
using Osta.Core.Feature.Chat.Command.Handler;
using Osta.Core.Feature.Chat.Command.Model;
using Osta.Core.HandlerMiddleware;
using Osta.Data.Entities.Booking;
using Osta.Domain.Entities.Chat;

namespace Osta.Test.ChatTesting.ChatCommandTesting
{
    public class SendMessageHandlerTesting
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IChatNotifier> chatNotifierMock;
        private readonly Mock<IBookingService> bookingServiceMock;
        private readonly Mock<IMessageService> messageServiceMock;
        private readonly SendMessageHandler handler;

        public SendMessageHandlerTesting()
        {
            mapperMock = new Mock<IMapper>();
            chatNotifierMock = new Mock<IChatNotifier>();
            messageServiceMock = new Mock<IMessageService>();
            bookingServiceMock = new Mock<IBookingService>();
            handler = new SendMessageHandler(mapperMock.Object,
                chatNotifierMock.Object,
                bookingServiceMock.Object, messageServiceMock.Object
                );

        }
        [Fact]
        public async Task Should_ThrowNotFoundException_When_BookingNotFound()
        {
            var command = new SendMessageCommand(1, "sender-id", "Content");


            bookingServiceMock.Setup
                (x => x.GetBookingById(command.BookingId, It.IsAny<CancellationToken>())).
                ReturnsAsync((Bookings?)null)
                ;

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal("Booking not found", exception.Message);

        }

        [Fact]
        public async Task Should_ThrowForbiddenException_When_SenderIsNotBookingParticipant()
        {
            var command = new SendMessageCommand(1, "sender-id", "Content")

            { BookingId = 1, SenderId = "unknown-user" };
            var booking = new Bookings { CustomerId = "customer-id", TechnicianId = "technician-id" };
            bookingServiceMock.Setup(x => x.GetBookingById(command.BookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

            var exception = await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal("Not a participant in this booking", exception.Message);
            messageServiceMock.Verify(x => x.SendMessage(It.IsAny<Message>(), CancellationToken.None), Times.Never)
                ; chatNotifierMock.Verify(x => x.NotifyNewMessage(It.IsAny<int>(), It.IsAny<MessageModel>()), Times.Never);
        }

        [Fact]
        public async Task Should_SendMessage_When_SenderIsCustomer()
        {
            var command = new SendMessageCommand(1, "customer-id", "Content");
            var booking = new Bookings { CustomerId = "customer-id", TechnicianId = "technician-id" };
            var message = new Message();
            var messageDto = new MessageModel();
            bookingServiceMock.Setup(x => x.GetBookingById(command.BookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
            mapperMock.Setup(x => x.Map<Message>(command)).Returns(message);
            mapperMock.Setup(x => x.Map<MessageModel>(message)).Returns(messageDto);
            messageServiceMock.Setup(x => x.SendMessage(message, CancellationToken.None)).Returns(Task.CompletedTask);
            chatNotifierMock.Setup(x => x.NotifyNewMessage(command.BookingId, messageDto)).Returns(Task.CompletedTask);
            var result = await handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result);
            //Assert.Equal(messageDto, result.Data);

            messageServiceMock.Verify(x => x.SendMessage(message, CancellationToken.None), Times.Once);
            chatNotifierMock.Verify(x => x.NotifyNewMessage(command.BookingId, messageDto), Times.Once);
        }

        [Fact]
        public async Task Should_SendMessage_When_SenderIsTechnician()
        {
            var command = new SendMessageCommand(1, "technician-id", "Content");

            ;
            var booking = new Bookings { CustomerId = "customer-id", TechnicianId = "technician-id" };
            var message = new Message();
            var messageDto = new MessageModel();
            bookingServiceMock.Setup(x => x.GetBookingById(command.BookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking); mapperMock.Setup(x => x.Map<Message>(command)).Returns(message); mapperMock.Setup(x => x.Map<MessageModel>(message)).Returns(messageDto); messageServiceMock.Setup(x => x.SendMessage(message, CancellationToken.None)).Returns(Task.CompletedTask);
            chatNotifierMock.Setup(x => x.NotifyNewMessage(command.BookingId, messageDto)).Returns(Task.CompletedTask);
            var result = await handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result); Assert.Equal(messageDto, result.Data);
            messageServiceMock.Verify(x => x.SendMessage(message, CancellationToken.None), Times.Once);
            chatNotifierMock.Verify(x => x.NotifyNewMessage(command.BookingId, messageDto), Times.Once);
        }
        [Fact]
        public async Task Should_MapCommandToMessage()
        {
            var command = new SendMessageCommand(1, "customer-id", "cont");

            ; var booking = new Bookings { CustomerId = "customer-id", TechnicianId = "technician-id" };
            var message = new Message();
            var messageDto = new MessageModel();
            bookingServiceMock.Setup(x => x.GetBookingById(command.BookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
            mapperMock.Setup(x => x.Map<Message>(command)).Returns(message);
            mapperMock.Setup(x => x.Map<MessageModel>(message)).Returns(messageDto);
            await handler.Handle(command, CancellationToken.None);
            mapperMock.Verify(x => x.Map<Message>(command), Times.Once); mapperMock.Verify(x => x.Map<MessageModel>(message), Times.Once);
        }
        [Fact]
        public async Task Should_NotifyNewMessage_AfterSendingMessage()
        {
            var command = new SendMessageCommand(1, "customer-id", "content");
            var booking = new Bookings { CustomerId = "customer-id", TechnicianId = "technician-id" };
            var message = new Message();
            var messageDto = new MessageModel();
            bookingServiceMock.Setup(x => x.GetBookingById(command.BookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
            mapperMock.Setup(x => x.Map<Message>(command)).Returns(message);
            mapperMock.Setup(x => x.Map<MessageModel>(message)).Returns(messageDto);
            messageServiceMock.Setup(x => x.SendMessage(message, CancellationToken.None)).Returns(Task.CompletedTask);
            chatNotifierMock.Setup(x => x.NotifyNewMessage(command.BookingId, messageDto)).Returns(Task.CompletedTask);
            await handler.Handle(command, CancellationToken.None);
            var sequence = new MockSequence(); messageServiceMock.InSequence(sequence).Setup(x => x.SendMessage(message, CancellationToken.None));
            chatNotifierMock.InSequence(sequence).Setup(x => x.NotifyNewMessage(command.BookingId, messageDto));
            messageServiceMock.Verify(x => x.SendMessage(message, CancellationToken.None), Times.Once);
            chatNotifierMock.Verify(x => x.NotifyNewMessage(command.BookingId, messageDto), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnSuccessResponse_When_MessageSentSuccessfully()
        {

            var command = new SendMessageCommand(1, "customer-id", "Content");


            var booking = new Bookings { CustomerId = "customer-id", TechnicianId = "technician-id" };
            var message = new Message();
            var messageDto = new MessageModel();
            bookingServiceMock.Setup(x => x.GetBookingById(command.BookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
            mapperMock.Setup(x => x.Map<Message>(command)).Returns(message);
            mapperMock.Setup(x => x.Map<MessageModel>(message)).Returns(messageDto);
            messageServiceMock.Setup(x => x.SendMessage(message, CancellationToken.None)).Returns(Task.CompletedTask);
            chatNotifierMock.Setup(x => x.NotifyNewMessage(command.BookingId, messageDto)).Returns(Task.CompletedTask);
            var result = await handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result); Assert.Equal(messageDto, result.Data);
        }
    }
}
