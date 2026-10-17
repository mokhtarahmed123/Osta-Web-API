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
    public class AddServiceAreaCommandTest
    {
        [Fact]
        public async Task Handle_ShouldReturnCreated_WhenServiceAreaAddedSuccessfully()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var command = new AddServiceAreaCommand
            {
                Name = "ServiceAreaName",
                City = "CityName",
                State = "StateName"
            };

            var serviceArea = new ServiceArea
            {
                Id = 1,
                Name = "ServiceAreaName",
                City = "CityName",
                State = "StateName"
            };

            mockMapper
                .Setup(x => x.Map<ServiceArea>(command))
                .Returns(serviceArea);

            mockServiceAreaService
                .Setup(x => x.AddServiceAreaAsync(serviceArea, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new AddServiceAreaCommandHandler(
                mockMapper.Object,
                mockLogger.Object,
                mockServiceAreaService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);

            mockMapper.Verify(
                x => x.Map<ServiceArea>(command),
                Times.Once);

            mockServiceAreaService.Verify(
                x => x.AddServiceAreaAsync(serviceArea, It.IsAny<CancellationToken>()),
                Times.Once);

        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenMapperThrowsException()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var command = new AddServiceAreaCommand();

            mockMapper
                .Setup(x => x.Map<ServiceArea>(command))
                .Throws(new Exception("Mapping Error"));

            var handler = new AddServiceAreaCommandHandler(
                mockMapper.Object,
                mockLogger.Object,
                mockServiceAreaService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            mockServiceAreaService.Verify(
                x => x.AddServiceAreaAsync(It.IsAny<ServiceArea>(), It.IsAny<CancellationToken>()),
                Times.Never);

            mockLogger.Verify(
                x => x.LogError(It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var command = new AddServiceAreaCommand();

            var serviceArea = new ServiceArea();

            mockMapper
                .Setup(x => x.Map<ServiceArea>(command))
                .Returns(serviceArea);

            mockServiceAreaService
                .Setup(x => x.AddServiceAreaAsync(serviceArea, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database Error"));

            var handler = new AddServiceAreaCommandHandler(
                mockMapper.Object,
                mockLogger.Object,
                mockServiceAreaService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            mockServiceAreaService.Verify(
    x => x.AddServiceAreaAsync(serviceArea, It.IsAny<CancellationToken>()),
        Times.Once);
            mockLogger.Verify(
                x => x.LogError(It.IsAny<string>()),
                Times.Once);

        }
    }
}
