using AutoMapper;
using Moq;
using Osta.Core.Feature.ServiceArea.Query.Handler;
using Osta.Core.Feature.ServiceArea.Query.Model;
using Osta.Core.Feature.ServiceArea.Query.Result;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;
using System.Net;

namespace Osta.Test.ServiceAreaTesting.Query
{
    public class GetServiceAreaByIdQueryTest
    {
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenServiceAreaExists()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var query = new GetServiceAreaByIdQuery(1);

            var serviceArea = new ServiceArea
            {
                Id = 1,
                Name = "Test Area",
                City = "Cairo",
                State = "Cairo"
            };

            var resultDto = new GetServiceAreaByIdResult
            {
                Id = 1,
                Name = "Test Area",
                City = "Cairo",
                State = "Cairo"
            };

            mockServiceAreaService
                .Setup(x => x.GetServiceAreaAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceArea);

            mockMapper
                .Setup(x => x.Map<GetServiceAreaByIdResult>(serviceArea))
                .Returns(resultDto);

            var handler = new ServiceAreaQueryHandler(
                mockMapper.Object,
                mockLogger.Object,
                mockServiceAreaService.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            mockServiceAreaService.Verify(
                x => x.GetServiceAreaAsync(query.Id, It.IsAny<CancellationToken>()),
                Times.Once);

            mockMapper.Verify(
                x => x.Map<GetServiceAreaByIdResult>(serviceArea),
                Times.Once);

            mockLogger.Verify(
                x => x.LogWarning(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenServiceAreaDoesNotExist()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var query = new GetServiceAreaByIdQuery(1);

            mockServiceAreaService
                .Setup(x => x.GetServiceAreaAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ServiceArea)null);

            var handler = new ServiceAreaQueryHandler(
                mockMapper.Object,
                mockLogger.Object,
                mockServiceAreaService.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

            mockServiceAreaService.Verify(
                x => x.GetServiceAreaAsync(query.Id, It.IsAny<CancellationToken>()),
                Times.Once);

            mockMapper.Verify(
                x => x.Map<GetServiceAreaByIdResult>(It.IsAny<ServiceArea>()),
                Times.Never);

            mockLogger.Verify(
                x => x.LogWarning($"Service Area not found with Id: {query.Id}"),
                Times.Once);
        }
    }
}