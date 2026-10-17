using AutoMapper;
using Moq;
using Osta.Core.Feature.Service.Query.Handler;
using Osta.Core.Feature.Service.Query.Model;
using Osta.Core.Feature.Service.Query.Result;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Test.ServiceTesting.Queries
{
    public class GetAllServiceQueryTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IServiceService> _serviceMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly ServiceQueryHandler _handler;

        public GetAllServiceQueryTest()
        {
            _mapperMock = new Mock<IMapper>();
            _serviceMock = new Mock<IServiceService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new ServiceQueryHandler(
                _mapperMock.Object,
                _serviceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllServices_WhenServicesExist()
        {
            // Arrange
            var query = new GetAllServicesQuery();

            var services = new List<Data.Entities.Services.Service>
            {
                new()
                {
                    Id = 1,
                    Name = "Electrical"
                },
                new()
                {
                    Id = 2,
                    Name = "Plumbing"
                }
            };

            var resultList = new List<GetAllServiceResult>
            {
                new()
                {
                    Id = 1,
                    Name = "Electrical"
                },
                new()
                {
                    Id = 2,
                    Name = "Plumbing"
                }
            };

            _serviceMock
                .Setup(x => x.GetAllServicesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(services);

            _mapperMock
                .Setup(x => x.Map<List<GetAllServiceResult>>(services))
                .Returns(resultList);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);

            _serviceMock.Verify(x =>
                x.GetAllServicesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoServicesExist()
        {
            // Arrange
            var query = new GetAllServicesQuery();

            var services = new List<Data.Entities.Services.Service>();

            var resultList = new List<GetAllServiceResult>();

            _serviceMock
                .Setup(x => x.GetAllServicesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(services);

            _mapperMock
                .Setup(x => x.Map<List<GetAllServiceResult>>(services))
                .Returns(resultList);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Empty(result.Data);

            _serviceMock.Verify(x =>
                x.GetAllServicesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}