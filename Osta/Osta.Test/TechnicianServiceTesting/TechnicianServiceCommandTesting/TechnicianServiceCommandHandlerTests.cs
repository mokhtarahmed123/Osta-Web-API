
using AutoMapper;
using Moq;
using Osta.Core.Feature.Technician.Command.Handler.TechnicianServiceCommandHandler;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Test.TechnicianServiceTesting.TechnicianServiceCommandTesting
{
    public class TechnicianServiceCommandHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITechnicianService> _technicianServiceMock;
        private readonly Mock<ILoggerService> _loggerServiceMock;
        private readonly Mock<ITechnicianServiceService> _technicianServiceServiceMock;
        private readonly Mock<ITechnicianServiceAreasService> _technicianServiceAreasServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;

        private readonly TechnicianServiceCommandHandler _handler;

        public TechnicianServiceCommandHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _technicianServiceMock = new Mock<ITechnicianService>();
            _loggerServiceMock = new Mock<ILoggerService>();
            _technicianServiceServiceMock = new Mock<ITechnicianServiceService>();
            _technicianServiceAreasServiceMock = new Mock<ITechnicianServiceAreasService>();
            _currentUserMock = new Mock<ICurrentUserService>();

            _handler = new TechnicianServiceCommandHandler(
                _mapperMock.Object,
                _technicianServiceMock.Object,
                _loggerServiceMock.Object,
                _technicianServiceServiceMock.Object,
                _technicianServiceAreasServiceMock.Object,
                _currentUserMock.Object);
        }

        [Fact]
        public async Task Handle_TechnicianNotFound_ShouldReturnBadRequest()
        {
            // Arrange
            const string technicianId = "tech-1";

            _currentUserMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(technicianId, CancellationToken.None))
                .ReturnsAsync((Technicians)null);

            var request = new TechnicianAddServiceCommand
            {
                ServiceIds = new List<int> { 1, 2 }
            };

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Succeeded);
            Assert.Equal(
                "An error occurred while processing your request.",
                response.Message);

            _technicianServiceServiceMock.Verify(
                x => x.AddRangeAsync(
                    It.IsAny<List<TechnicianService>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _loggerServiceMock.Verify(
                x => x.LogError(
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ValidTechnician_ShouldAddServicesSuccessfully()
        {
            // Arrange
            const string technicianId = "tech-1";

            _currentUserMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var technician = new Technicians();

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(technicianId, CancellationToken.None))
                .ReturnsAsync(technician);

            var request = new TechnicianAddServiceCommand
            {
                ServiceIds = new List<int> { 1, 2, 3 }
            };

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Succeeded);
            //Assert.Contains(
            //    $"Services For Technician With Id {technicianId} Added successfully",
            //    response.Message);

            _technicianServiceServiceMock.Verify(
                x => x.AddRangeAsync(
                    It.Is<List<TechnicianService>>(services =>
                        services.Count == 3 &&
                        services.All(x => x.TechnicianId == technicianId) &&
                        services[0].ServiceId == 1 &&
                        services[1].ServiceId == 2 &&
                        services[2].ServiceId == 3),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _loggerServiceMock.Verify(
                x => x.LogInformation(
                    It.Is<string>(message =>
                        message.Contains(technicianId))),
                Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyServiceIds_ShouldAddEmptyListSuccessfully()
        {
            // Arrange
            const string technicianId = "tech-1";

            _currentUserMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var technician = new Technicians();

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(technicianId, CancellationToken.None))
                .ReturnsAsync(technician);

            var request = new TechnicianAddServiceCommand
            {
                ServiceIds = new List<int>()
            };

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Succeeded);

            _technicianServiceServiceMock.Verify(
                x => x.AddRangeAsync(
                    It.Is<List<TechnicianService>>(services =>
                        services.Count == 0),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_AddRangeThrowsException_ShouldReturnBadRequest()
        {
            // Arrange
            const string technicianId = "tech-1";

            _currentUserMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var technician = new Technicians();

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(technicianId, CancellationToken.None))
                .ReturnsAsync(technician);

            _technicianServiceServiceMock
                .Setup(x => x.AddRangeAsync(
                    It.IsAny<List<TechnicianService>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var request = new TechnicianAddServiceCommand
            {
                ServiceIds = new List<int> { 1, 2 }
            };

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Succeeded);
            Assert.Equal(
                "An error occurred while processing your request.",
                response.Message);

            _loggerServiceMock.Verify(
                x => x.LogError(
                    It.IsAny<Exception>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldCreateCorrectTechnicianServices()
        {
            // Arrange
            const string technicianId = "tech-123";

            _currentUserMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(technicianId, CancellationToken.None))
                .ReturnsAsync(new Technicians());

            var request = new TechnicianAddServiceCommand
            {
                ServiceIds = new List<int> { 10, 20 }
            };

            List<TechnicianService> capturedServices = null;

            _technicianServiceServiceMock
                .Setup(x => x.AddRangeAsync(
                    It.IsAny<List<TechnicianService>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<TechnicianService>, CancellationToken>(
                    (services, _) => capturedServices = services.ToList())
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(capturedServices);
            Assert.Equal(2, capturedServices.Count);

            Assert.Equal(
                technicianId,
                capturedServices[0].TechnicianId);

            Assert.Equal(
                technicianId,
                capturedServices[1].TechnicianId);

            Assert.Equal(10, capturedServices[0].ServiceId);
            Assert.Equal(20, capturedServices[1].ServiceId);
        }
    }
}

