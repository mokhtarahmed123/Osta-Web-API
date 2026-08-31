using AutoMapper;
using Moq;
using Osta.Chat.MessageService;
using Osta.Chat.Model;
using Osta.Core.Feature.Chat.Query.Handler;
using Osta.Core.Feature.Chat.Query.Model;
using Osta.Domain.Entities.Chat;

namespace Osta.Test.ChatTesting.ChatQueryTesting
{
    public class GetBookingMessagesHandlerTests
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IMessageService> messageServiceMock;
        private readonly GetBookingMessagesHandler handler;

        public GetBookingMessagesHandlerTests()
        {
            mapperMock = new Mock<IMapper>();
            messageServiceMock = new Mock<IMessageService>();

            handler = new GetBookingMessagesHandler(
                mapperMock.Object,
                messageServiceMock.Object);
        }


        [Fact]
        public async Task Should_ReturnMessages_When_BookingHasMessages()
        {
            // Arrange
            int bookingId = 1;

            var messages = new List<Message>
            {
                new Message(),
                new Message()
            };

            var messageDtos = new List<MessageModel>
            {
                new MessageModel(),
                new MessageModel()
            };

            var query = new GetBookingMessagesQuery(bookingId);

            messageServiceMock
                .Setup(x => x.GetMessageByBookingId(bookingId, CancellationToken.None))
                .ReturnsAsync(messages);

            mapperMock
                .Setup(x => x.Map<List<MessageModel>>(messages))
                .Returns(messageDtos);

            // Act
            var result = await handler.Handle(
                query,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(messageDtos, result.Data);

            Assert.Equal(2, result.Data.Count);
        }


        [Fact]
        public async Task Should_ReturnEmptyList_When_BookingHasNoMessages()
        {
            // Arrange
            int bookingId = 1;

            var messages = new List<Message>();
            var messageDtos = new List<MessageModel>();

            var query = new GetBookingMessagesQuery(bookingId);

            messageServiceMock
                .Setup(x => x.GetMessageByBookingId(bookingId, CancellationToken.None))
                .ReturnsAsync(messages);

            mapperMock
                .Setup(x => x.Map<List<MessageModel>>(messages))
                .Returns(messageDtos);

            // Act
            var result = await handler.Handle(
                query,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }


        [Fact]
        public async Task Should_CallGetMessageByBookingId_Once()
        {
            // Arrange
            int bookingId = 1;

            var messages = new List<Message>();
            var messageDtos = new List<MessageModel>();

            var query = new GetBookingMessagesQuery(bookingId);


            messageServiceMock
                .Setup(x => x.GetMessageByBookingId(bookingId, CancellationToken.None))
                .ReturnsAsync(messages);

            mapperMock
                .Setup(x => x.Map<List<MessageModel>>(messages))
                .Returns(messageDtos);

            // Act
            await handler.Handle(
                query,
                CancellationToken.None);

            // Assert
            messageServiceMock.Verify(
                x => x.GetMessageByBookingId(bookingId, CancellationToken.None),
                Times.Once);
        }


        [Fact]
        public async Task Should_MapMessagesToMessageDtos()
        {
            // Arrange
            int bookingId = 1;

            var messages = new List<Message>
            {
                new Message()
            };

            var messageDtos = new List<MessageModel>
            {
                new MessageModel()
            };

            var query = new GetBookingMessagesQuery(bookingId);

            messageServiceMock
                .Setup(x => x.GetMessageByBookingId(bookingId, CancellationToken.None))
                .ReturnsAsync(messages);

            mapperMock
                .Setup(x => x.Map<List<MessageModel>>(messages))
                .Returns(messageDtos);

            // Act
            await handler.Handle(
                query,
                CancellationToken.None);

            // Assert
            mapperMock.Verify(
                x => x.Map<List<MessageModel>>(messages),
                Times.Once);
        }


        [Fact]
        public async Task Should_ReturnCorrectMappedData()
        {
            // Arrange
            int bookingId = 1;

            var messages = new List<Message>
            {
                new Message()
            };

            var messageDtos = new List<MessageModel>
            {
                new MessageModel()
            };

            var query = new GetBookingMessagesQuery(bookingId)
;
            messageServiceMock
                .Setup(x => x.GetMessageByBookingId(bookingId, CancellationToken.None))
                .ReturnsAsync(messages);

            mapperMock
                .Setup(x => x.Map<List<MessageModel>>(messages))
                .Returns(messageDtos);

            // Act
            var result = await handler.Handle(
                query,
                CancellationToken.None);

            // Assert
            Assert.Same(messageDtos, result.Data);
        }
    }
}

