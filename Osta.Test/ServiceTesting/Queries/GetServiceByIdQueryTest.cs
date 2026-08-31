using AutoMapper;
using Moq;
using Osta.Core.Feature.Service.Query.Handler;
using Osta.Core.Feature.Service.Query.Model;
using Osta.Core.Feature.Service.Query.Result;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Test.ServiceTesting.Queries
{
    public class GetServiceByIdQueryTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IServiceService> _serviceMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly ServiceQueryHandler _handler;

        public GetServiceByIdQueryTest()
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
        public async Task Handle_ShouldReturnService_WhenServiceExists()
        {
            // Arrange
            var query = new GetServiceByIdQuery(1);

            var service = new Data.Entities.Services.Service
            {
                Id = 1,
                Name = "Electrical"
            };

            var serviceResult = new GetServiceByIdResult
            {
                Id = 1,
                Name = "Electrical"
            };

            _serviceMock
                .Setup(x => x.GetServiceAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(service);

            _mapperMock
                .Setup(x => x.Map<GetServiceByIdResult>(service))
                .Returns(serviceResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(serviceResult.Id, result.Data.Id);
            Assert.Equal(serviceResult.Name, result.Data.Name);

            _serviceMock.Verify(x =>
                x.GetServiceAsync(query.Id, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenServiceDoesNotExist()
        {
            // Arrange
            var query = new GetServiceByIdQuery(100);

            _serviceMock
                .Setup(x => x.GetServiceAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Data.Entities.Services.Service?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Service not found.", result.Message);

            _mapperMock.Verify(x =>
                x.Map<GetServiceByIdResult>(It.IsAny<Data.Entities.Services.Service>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var query = new GetServiceByIdQuery(1);

            _serviceMock
                .Setup(x => x.GetServiceAsync(query.Id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database Error"));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("An error occurred while processing your request.", result.Message);

            _mapperMock.Verify(x =>
                x.Map<GetServiceByIdResult>(It.IsAny<Data.Entities.Services.Service>()),
                Times.Never);
        }
    }
}