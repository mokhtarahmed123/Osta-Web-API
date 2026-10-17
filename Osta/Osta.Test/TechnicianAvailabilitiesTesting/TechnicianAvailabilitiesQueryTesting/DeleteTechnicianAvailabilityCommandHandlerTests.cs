using Moq;
using Osta.Core.Feature.Technician.Command.Handler.TechnicianAvailabilitiesCommandHandler;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianAvailabilities;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Test.TechnicianAvailabilitiesTesting.TechnicianAvailabilitiesQueryTesting
{
    public class DeleteTechnicianAvailabilityCommandHandlerTests
    {

        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<ITechnicianAvailabilityService> _availabilityServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;

        private readonly DeleteTechnicianAvailabilityCommandHandler _handler;

        public DeleteTechnicianAvailabilityCommandHandlerTests()
        {

            _loggerMock = new Mock<ILoggerService>();
            _availabilityServiceMock = new Mock<ITechnicianAvailabilityService>();
            _currentUserMock = new Mock<ICurrentUserService>();

            _currentUserMock
                .Setup(x => x.UserId)
                .Returns("tech-123");

            _handler = new DeleteTechnicianAvailabilityCommandHandler(

                _loggerMock.Object,
                _availabilityServiceMock.Object,
                _currentUserMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnDeleted_WhenAvailabilityExists()
        {
            // Arrange
            var request = new DeleteTechnicianAvailabilityCommand(1);

            var availability = new TechnicianAvailability
            {
                Id = 1,
                TechnicianId = "tech-123"
            };

            _availabilityServiceMock
                .Setup(x => x.GetTechnicianAvailabilityForTechnicianAsync(
                    request.Id,
                    "tech-123",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(availability);

            _availabilityServiceMock
                .Setup(x => x.DeleteTechnicianAvailabilityAsync(
                    request.Id,
                    "tech-123",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(
                "Availability deleted successfully.",
                result.Message);

            _availabilityServiceMock.Verify(
                x => x.GetTechnicianAvailabilityForTechnicianAsync(
                    1,
                    "tech-123",
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _availabilityServiceMock.Verify(
                x => x.DeleteTechnicianAvailabilityAsync(
                    1,
                    "tech-123",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenAvailabilityDoesNotExist()
        {
            // Arrange
            var request = new DeleteTechnicianAvailabilityCommand(1);

            _availabilityServiceMock
                .Setup(x => x.GetTechnicianAvailabilityForTechnicianAsync(
                    request.Id,
                    "tech-123",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((TechnicianAvailability?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(
                "Availability not found.",
                result.Message);

            _availabilityServiceMock.Verify(
                x => x.GetTechnicianAvailabilityForTechnicianAsync(
                    1,
                    "tech-123",
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _availabilityServiceMock.Verify(
                x => x.DeleteTechnicianAvailabilityAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldUseCurrentUserTechnicianId()
        {
            // Arrange
            var request = new DeleteTechnicianAvailabilityCommand(5);

            var availability = new TechnicianAvailability
            {
                Id = 5,
                TechnicianId = "tech-123"
            };

            _availabilityServiceMock
                .Setup(x => x.GetTechnicianAvailabilityForTechnicianAsync(
                    5,
                    "tech-123",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(availability);

            _availabilityServiceMock
                .Setup(x => x.DeleteTechnicianAvailabilityAsync(
                    5,
                    "tech-123",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _availabilityServiceMock.Verify(
                x => x.GetTechnicianAvailabilityForTechnicianAsync(
                    5,
                    "tech-123",
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _availabilityServiceMock.Verify(
                x => x.DeleteTechnicianAvailabilityAsync(
                    5,
                    "tech-123",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
