
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
    public class UpdateTechnicianAvailabilityCommandHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<ITechnicianAvailabilityService> _availabilityServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;

        private readonly UpdateTechnicianAvailabilityCommandHandler _handler;

        public UpdateTechnicianAvailabilityCommandHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _availabilityServiceMock = new Mock<ITechnicianAvailabilityService>();
            _currentUserMock = new Mock<ICurrentUserService>();

            _currentUserMock
                .Setup(x => x.UserId)
                .Returns("tech-123");

            _handler = new UpdateTechnicianAvailabilityCommandHandler(
                _mapperMock.Object,
                _loggerMock.Object,
                _availabilityServiceMock.Object,
                _currentUserMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnUpdated_WhenAvailabilityExists()
        {
            // Arrange
            var request = new UpdateTechnicianAvailabilityCommand(1)
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsAvailable = true
            };

            var availability = new TechnicianAvailability
            {
                Id = 1,
                TechnicianId = "old-tech"
            };

            _availabilityServiceMock
                .Setup(x => x.GetTechnicianAvailabilityAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(availability);

            _mapperMock
                .Setup(x => x.Map(
                    request,
                    availability))
                .Callback<UpdateTechnicianAvailabilityCommand, TechnicianAvailability>(
                    (src, dest) =>
                    {
                        dest.DayOfWeek = src.DayOfWeek;
                        dest.StartTime = src.StartTime;
                        dest.EndTime = src.EndTime;
                        dest.IsAvailable = src.IsAvailable;
                    });

            _availabilityServiceMock
                .Setup(x => x.UpdateTechnicianAvailabilityAsync(
                    request.Id,
                    availability, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);

            Assert.Equal(
                "Technician availability updated successfully.",
                result.Message);

            Assert.Equal(
                "tech-123",
                availability.TechnicianId);

            _availabilityServiceMock.Verify(
                x => x.GetTechnicianAvailabilityAsync(
                    1,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map(
                    request,
                    availability),
                Times.Once);

            _availabilityServiceMock.Verify(
                x => x.UpdateTechnicianAvailabilityAsync(
                    1,
                    availability, CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenAvailabilityDoesNotExist()
        {
            // Arrange
            var request = new UpdateTechnicianAvailabilityCommand(1)
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsAvailable = true
            };

            _availabilityServiceMock
                .Setup(x => x.GetTechnicianAvailabilityAsync(
                    request.Id,
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
                "Technician availability not found.",
                result.Message);

            _mapperMock.Verify(
                x => x.Map(
                    It.IsAny<UpdateTechnicianAvailabilityCommand>(),
                    It.IsAny<TechnicianAvailability>()),
                Times.Never);

            _availabilityServiceMock.Verify(
                x => x.UpdateTechnicianAvailabilityAsync(
                    It.IsAny<int>(),
                    It.IsAny<TechnicianAvailability>(), CancellationToken.None),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldSetTechnicianId_FromCurrentUser()
        {
            // Arrange
            var request = new UpdateTechnicianAvailabilityCommand(10)
            {
                DayOfWeek = DayOfWeek.Friday,
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(18, 0),
                IsAvailable = true
            };

            var availability = new TechnicianAvailability
            {
                Id = 10,
                TechnicianId = "old-tech"
            };

            _availabilityServiceMock
                .Setup(x => x.GetTechnicianAvailabilityAsync(
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(availability);

            _mapperMock
                .Setup(x => x.Map(
                    request,
                    availability));

            _availabilityServiceMock
                .Setup(x => x.UpdateTechnicianAvailabilityAsync(
                    10,
                    availability, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "tech-123",
                availability.TechnicianId);

            _availabilityServiceMock.Verify(
                x => x.UpdateTechnicianAvailabilityAsync(
                    10,
                    It.Is<TechnicianAvailability>(a =>
                        a.TechnicianId == "tech-123"), CancellationToken.None),
                Times.Once);
        }
    }
}

