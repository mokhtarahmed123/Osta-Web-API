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
    public class GetAllServiceAreaQueryTest
    {
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenServiceAreasExist()
        {
            // Arrange
            var mockServiceAreaService = new Mock<IServiceAreaService>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILoggerService>();

            var query = new GetAllServiceAreasQuery();

            var serviceAreas = new List<ServiceArea>
    {
        new ServiceArea
        {
            Id = 1,
            Name = "Area 1",
            City = "Cairo",
            State = "Cairo"
        },
        new ServiceArea
        {
            Id = 2,
            Name = "Area 2",
            City = "Giza",
            State = "Giza"
        }
    };

            var resultDto = new List<GetAllServiceAreasResult>
    {
        new GetAllServiceAreasResult
        {
            Id = 1,
            Name = "Area 1",
            City = "Cairo",
            State = "Cairo"
        },
        new GetAllServiceAreasResult
        {
            Id = 2,
            Name = "Area 2",
            City = "Giza",
            State = "Giza"
        }
    };

            mockServiceAreaService
                .Setup(x => x.GetAllServiceAreasAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceAreas);

            mockMapper
                .Setup(x => x.Map<List<GetAllServiceAreasResult>>(serviceAreas))
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
                x => x.GetAllServiceAreasAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            mockMapper.Verify(
                x => x.Map<List<GetAllServiceAreasResult>>(serviceAreas),
                Times.Once);
        }

    }
}
