using Moq;
using Osta.Core.Feature.Service.Command.Handler;
using Osta.Core.Feature.Service.Command.Model;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Test.ServiceTesting.Command
{
    public class DeleteServiceCommandTest
    {

        private readonly Mock<IServiceService> _serviceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly DeleteServiceCommandHandler _handler;

        public DeleteServiceCommandTest()
        {

            _serviceMock = new Mock<IServiceService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new DeleteServiceCommandHandler(

                    _serviceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenIdIsLessThanOrEqualZero()
        {
            // Arrange
            var command = new DeleteServiceCommand(0)
            {
                Id = 0
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid service ID.", result.Message);

            _serviceMock.Verify(x =>
                x.DeleteServiceAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenServiceDoesNotExist()
        {
            // Arrange
            var command = new DeleteServiceCommand(5)
            {
                Id = 5
            };

            _serviceMock
                .Setup(x => x.GetServiceAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Data.Entities.Services.Service?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Service not found.", result.Message);

            _serviceMock.Verify(x =>
                x.DeleteServiceAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnDeleted_WhenServiceDeletedSuccessfully()
        {
            // Arrange
            var command = new DeleteServiceCommand(1)
            {
                Id = 1
            };

            var service = new Data.Entities.Services.Service
            {
                Id = 1
            };

            _serviceMock
                .Setup(x => x.GetServiceAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(service);

            _serviceMock
                .Setup(x => x.DeleteServiceAsync(command.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Service deleted successfully.", result.Message);

            _serviceMock.Verify(x =>
                x.DeleteServiceAsync(command.Id, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var command = new DeleteServiceCommand(1)
            {
                Id = 1
            };

            var service = new Data.Entities.Services.Service
            {
                Id = 1
            };

            _serviceMock
                .Setup(x => x.GetServiceAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(service);

            _serviceMock
                .Setup(x => x.DeleteServiceAsync(command.Id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database Error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("An error occurred while processing your request.", result.Message);

            _serviceMock.Verify(x =>
                x.DeleteServiceAsync(command.Id, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}