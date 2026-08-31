using AutoMapper;
using Moq;
using Osta.Core.Feature.Technician.Command.Handler.TechnicianAvailabilitiesCommandHandler;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianAvailabilities;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Test.TechnicianAvailabilitiesTesting.TechnicianAvailabilitiesQueryTesting
{
    public class AddTechnicianAvailabilityCommandHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<ITechnicianAvailabilityService> _availabilityServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;

        private readonly AddTechnicianAvailabilityCommandHandler _handler;

        public AddTechnicianAvailabilityCommandHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _availabilityServiceMock = new Mock<ITechnicianAvailabilityService>();
            _currentUserMock = new Mock<ICurrentUserService>();

            _currentUserMock
                .Setup(x => x.UserId)
                .Returns("tech-123");

            _handler = new AddTechnicianAvailabilityCommandHandler(
                _mapperMock.Object,
                _loggerMock.Object,
                _availabilityServiceMock.Object,
                _currentUserMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCreated_WhenAvailabilityAddedSuccessfully()
        {
            // Arrange
            var request = new RequestTechnicianAvailabilityCommand
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsAvailable = true
            };

            var availability = new TechnicianAvailability();

            _mapperMock
                .Setup(x => x.Map<TechnicianAvailability>(request))
                .Returns(availability);

            _availabilityServiceMock
                .Setup(x => x.AddTechnicianAvailabilityAsync(
                    It.IsAny<TechnicianAvailability>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            //Assert.Equal(
            //    "Availability added successfully.",
            //    result.Message);

            Assert.Equal(
                "tech-123",
                availability.TechnicianId);

            _mapperMock.Verify(
                x => x.Map<TechnicianAvailability>(request),
                Times.Once);

            _availabilityServiceMock.Verify(
                x => x.AddTechnicianAvailabilityAsync(
                    It.Is<TechnicianAvailability>(a =>
                        a.TechnicianId == "tech-123"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var request = new RequestTechnicianAvailabilityCommand
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsAvailable = true
            };

            var availability = new TechnicianAvailability();

            _mapperMock
                .Setup(x => x.Map<TechnicianAvailability>(request))
                .Returns(availability);

            _availabilityServiceMock
                .Setup(x => x.AddTechnicianAvailabilityAsync(
                    It.IsAny<TechnicianAvailability>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(
                "Failed to add availability.",
                result.Message);

            _availabilityServiceMock.Verify(
                x => x.AddTechnicianAvailabilityAsync(
                    It.IsAny<TechnicianAvailability>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldSetTechnicianId_FromCurrentUser()
        {
            // Arrange
            var request = new RequestTechnicianAvailabilityCommand
            {
                DayOfWeek = DayOfWeek.Friday,
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(18, 0),
                IsAvailable = true
            };

            var availability = new TechnicianAvailability();

            _mapperMock
                .Setup(x => x.Map<TechnicianAvailability>(request))
                .Returns(availability);

            _availabilityServiceMock
                .Setup(x => x.AddTechnicianAvailabilityAsync(
                    It.IsAny<TechnicianAvailability>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "tech-123",
                availability.TechnicianId);
        }
    }
}