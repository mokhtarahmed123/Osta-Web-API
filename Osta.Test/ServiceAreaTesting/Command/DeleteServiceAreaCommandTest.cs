using AutoMapper;
using Moq;
using Osta.Core.Feature.ServiceArea.Command.Handler;
using Osta.Core.Feature.ServiceArea.Command.Model;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;
using System.Net;

namespace Osta.Test.ServiceAreaTesting.Command
{
    public class DeleteServiceAreaCommandTest
    {
        [Fact]
        public async Task Handle_ShouldReturnDeleted_WhenServiceAreaDeletedSuccessfully()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var command = new DeleteServiceAreaCommand(1);

            mockServiceAreaService
                .Setup(x => x.GetServiceAreaAsync(command.id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceArea { Id = 1 });

            mockServiceAreaService
                .Setup(x => x.DeleteServiceAreaAsync(command.id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new DeleteServiceAreaCommandHandler(
                mockMapper.Object,
                mockLogger.Object,
                mockServiceAreaService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            mockServiceAreaService.Verify(
                x => x.GetServiceAreaAsync(command.id, It.IsAny<CancellationToken>()),
                Times.Once);

            mockServiceAreaService.Verify(
                x => x.DeleteServiceAreaAsync(command.id, It.IsAny<CancellationToken>()),
                Times.Once);

            mockLogger.Verify(
                x => x.LogInformation($"Service Area  deleted successfully. Id: {command.id}"),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenServiceAreaDoesNotExist()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var command = new DeleteServiceAreaCommand(1);

            mockServiceAreaService
                .Setup(x => x.GetServiceAreaAsync(command.id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ServiceArea)null);

            var handler = new DeleteServiceAreaCommandHandler(
                mockMapper.Object,
                mockLogger.Object,
                mockServiceAreaService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

            mockServiceAreaService.Verify(
                x => x.DeleteServiceAreaAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
                Times.Never);

            mockLogger.Verify(
                x => x.LogError($"Service Area   With Id {command.id} Not Found"),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenDeleteThrowsException()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var command = new DeleteServiceAreaCommand(1);

            mockServiceAreaService
                .Setup(x => x.GetServiceAreaAsync(command.id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceArea { Id = 1 });

            mockServiceAreaService
                .Setup(x => x.DeleteServiceAreaAsync(command.id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Delete Error"));

            var handler = new DeleteServiceAreaCommandHandler(
                mockMapper.Object,
                mockLogger.Object,
                mockServiceAreaService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            mockServiceAreaService.Verify(
                x => x.DeleteServiceAreaAsync(command.id, It.IsAny<CancellationToken>()),
                Times.Once);

            mockLogger.Verify(
                x => x.LogError(It.IsAny<string>()),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenGetServiceAreaThrowsException()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var command = new DeleteServiceAreaCommand(1);

            mockServiceAreaService
                .Setup(x => x.GetServiceAreaAsync(command.id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database Error"));

            var handler = new DeleteServiceAreaCommandHandler(
                mockMapper.Object,
                mockLogger.Object,
                mockServiceAreaService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            mockServiceAreaService.Verify(
                x => x.DeleteServiceAreaAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
                Times.Never);

            mockLogger.Verify(
                x => x.LogError(It.IsAny<string>()),
                Times.Once);
        }
    }
}
