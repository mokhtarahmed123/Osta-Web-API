using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using Osta.Core.Feature.Service.Command.Handler;
using Osta.Core.Feature.Service.Command.Model;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Test.ServiceTesting.Command
{
    public class UpdateServiceCommandTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IServiceService> _serviceMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly UpdateServiceCommandHandler _handler;

        public UpdateServiceCommandTest()
        {
            _mapperMock = new Mock<IMapper>();
            _serviceMock = new Mock<IServiceService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new UpdateServiceCommandHandler(
                _mapperMock.Object,
                _serviceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenServiceDoesNotExist()
        {
            // Arrange
            var command = new UpdateServiceCommand(1)
            {
                Name = "Electrical",
                Description = "Electrical Service",
                Price = 100,
                CategoryId = 1,
                IsActive = true
            };

            _serviceMock
                .Setup(x => x.GetServiceAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Data.Entities.Services.Service?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Service not found.", result.Message);

            _serviceMock.Verify(x => x.UpdateServiceAsync(
                It.IsAny<int>(),
                It.IsAny<Data.Entities.Services.Service>(),
                It.IsAny<IFormFile>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUpdated_WhenServiceUpdatedSuccessfully()
        {
            // Arrange
            var command = new UpdateServiceCommand(1)
            {
                Name = "Electrical",
                Description = "Electrical Service",
                Price = 100,
                CategoryId = 1,
                Image = null,
                IsActive = true
            };

            var existingService = new Data.Entities.Services.Service
            {
                Id = 1,
                Name = "Old Service"
            };

            var updatedService = new Data.Entities.Services.Service
            {
                Id = 1,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                CategoryId = command.CategoryId,
                IsActive = command.IsActive
            };

            _serviceMock
                .Setup(x => x.GetServiceAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingService);

            _mapperMock
                .Setup(x => x.Map<Data.Entities.Services.Service>(command))
                .Returns(updatedService);

            _serviceMock
                .Setup(x => x.UpdateServiceAsync(
                    command.Id,
                    updatedService,
                    command.Image,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Service updated successfully.", result.Message);

            _serviceMock.Verify(x => x.UpdateServiceAsync(
                command.Id,
                updatedService,
                command.Image,
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var command = new UpdateServiceCommand(1)
            {
                Name = "Electrical",
                Description = "Electrical Service",
                Price = 100,
                CategoryId = 1,
                Image = null,
                IsActive = true
            };

            var existingService = new Data.Entities.Services.Service
            {
                Id = 1
            };

            var updatedService = new Data.Entities.Services.Service
            {
                Id = 1
            };

            _serviceMock
                .Setup(x => x.GetServiceAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingService);

            _mapperMock
                .Setup(x => x.Map<Data.Entities.Services.Service>(command))
                .Returns(updatedService);

            _serviceMock
                .Setup(x => x.UpdateServiceAsync(
                    It.IsAny<int>(),
                    It.IsAny<Data.Entities.Services.Service>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database Error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("An error occurred while processing your request.", result.Message);

            _serviceMock.Verify(x => x.UpdateServiceAsync(
                It.IsAny<int>(),
                It.IsAny<Data.Entities.Services.Service>(),
                It.IsAny<IFormFile>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}