using AutoMapper;
using Moq;
using Osta.Core.Feature.Service.Command.Handler;
using Osta.Core.Feature.Service.Command.Model;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Test.ServiceTesting.Command
{
    public class AddServiceCommandTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IServiceService> _serviceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly AddServiceCommandHandler _handler;

        public AddServiceCommandTest()
        {
            _mapperMock = new Mock<IMapper>();
            _serviceMock = new Mock<IServiceService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new AddServiceCommandHandler(
                _mapperMock.Object,
                _serviceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCreated_WhenServiceAddedSuccessfully()
        {
            // Arrange
            var command = new AddServiceCommand
            {
                Name = "Electrical",
                Description = "Electrical Service",
                Price = 100,
                CategoryId = 1,
                Image = null,
                IsActive = true
            };

            var service = new Data.Entities.Services.Service
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                CategoryId = command.CategoryId,
                IsActive = command.IsActive
            };

            _mapperMock
                .Setup(x => x.Map<Data.Entities.Services.Service>(command))
                .Returns(service);

            _serviceMock
                .Setup(x => x.AddServiceAsync(
                    service,
                    command.Image,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Service created successfully.", result.Data);
            Assert.Equal(" created successfully.", result.Message);
            _serviceMock.Verify(x => x.AddServiceAsync(
                service,
                command.Image,
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var command = new AddServiceCommand
            {
                Name = "Electrical",
                Description = "Electrical Service",
                Price = 100,
                CategoryId = 1
            };

            var service = new Data.Entities.Services.Service();

            _mapperMock
                .Setup(x => x.Map<Data.Entities.Services.Service>(command))
                .Returns(service);

            _serviceMock
                .Setup(x => x.AddServiceAsync(
                    It.IsAny<Data.Entities.Services.Service>(),
                    It.IsAny<Microsoft.AspNetCore.Http.IFormFile>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("An error occurred while processing your request.", result.Message);

            _serviceMock.Verify(x => x.AddServiceAsync(
                It.IsAny<Data.Entities.Services.Service>(),
                It.IsAny<Microsoft.AspNetCore.Http.IFormFile>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}