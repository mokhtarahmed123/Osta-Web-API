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
    public class UpdateServiceAreaCommandTest
    {
        [Fact]
        public async Task Handle_ShouldReturnUpdated_WhenServiceAreaUpdatedSuccessfully()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var command = new UpdateServiceAreaCommand(1)
            {
                Id = 1,
                Name = "Updated Name",
                City = "Cairo",
                State = "Cairo"
            };

            var existingServiceArea = new ServiceArea { Id = 1 };

            var updatedServiceArea = new ServiceArea
            {
                Id = 1,
                Name = "Updated Name",
                City = "Cairo",
                State = "Cairo"
            };

            mockServiceAreaService
                .Setup(x => x.GetServiceAreaAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingServiceArea);

            mockMapper
                .Setup(x => x.Map<ServiceArea>(command))
                .Returns(updatedServiceArea);

            mockServiceAreaService
                .Setup(x => x.UpdateServiceAreaAsync(command.Id, updatedServiceArea, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new UpdateServiceAreaCommandHandler(
                mockMapper.Object,
                mockLogger.Object,
                mockServiceAreaService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            mockServiceAreaService.Verify(
                x => x.GetServiceAreaAsync(command.Id, It.IsAny<CancellationToken>()),
                Times.Once);

            mockMapper.Verify(
                x => x.Map<ServiceArea>(command),
                Times.Once);

            mockServiceAreaService.Verify(
                x => x.UpdateServiceAreaAsync(command.Id, updatedServiceArea, It.IsAny<CancellationToken>()),
                Times.Once);

            mockLogger.Verify(
                x => x.LogInformation($"Service Area updated successfully. Id: {command.Id}"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenServiceAreaDoesNotExist()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var command = new UpdateServiceAreaCommand(1)
            {
                Id = 1
            };

            mockServiceAreaService
                .Setup(x => x.GetServiceAreaAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ServiceArea)null);

            var handler = new UpdateServiceAreaCommandHandler(
                mockMapper.Object,
                mockLogger.Object,
                mockServiceAreaService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

            mockMapper.Verify(
                x => x.Map<ServiceArea>(It.IsAny<UpdateServiceAreaCommand>()),
                Times.Never);

            mockServiceAreaService.Verify(
                x => x.UpdateServiceAreaAsync(It.IsAny<int>(), It.IsAny<ServiceArea>(), It.IsAny<CancellationToken>()),
                Times.Never);

            mockLogger.Verify(
                x => x.LogError(It.IsAny<string>()),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenMapperThrowsException()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var command = new UpdateServiceAreaCommand(1)
            {
                Id = 1
            };

            mockServiceAreaService
                .Setup(x => x.GetServiceAreaAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceArea { Id = 1 });

            mockMapper
                .Setup(x => x.Map<ServiceArea>(command))
                .Throws(new Exception("Mapping Error"));

            var handler = new UpdateServiceAreaCommandHandler(
                mockMapper.Object,
                mockLogger.Object,
                mockServiceAreaService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            mockServiceAreaService.Verify(
                x => x.UpdateServiceAreaAsync(It.IsAny<int>(), It.IsAny<ServiceArea>(), It.IsAny<CancellationToken>()),
                Times.Never);

            mockLogger.Verify(
                x => x.LogError(It.IsAny<string>()),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenUpdateServiceThrowsException()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var command = new UpdateServiceAreaCommand(1)
            {
                Id = 1
            };

            var existing = new ServiceArea { Id = 1 };
            var updated = new ServiceArea { Id = 1 };

            mockServiceAreaService
                .Setup(x => x.GetServiceAreaAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            mockMapper
                .Setup(x => x.Map<ServiceArea>(command))
                .Returns(updated);

            mockServiceAreaService
                .Setup(x => x.UpdateServiceAreaAsync(command.Id, updated, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database Error"));

            var handler = new UpdateServiceAreaCommandHandler(
                mockMapper.Object,
                mockLogger.Object,
                mockServiceAreaService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            mockServiceAreaService.Verify(
                x => x.UpdateServiceAreaAsync(command.Id, updated, It.IsAny<CancellationToken>()),
                Times.Once);

            mockLogger.Verify(
                x => x.LogError(It.IsAny<string>()),
                Times.Once);
        }
    }
}
