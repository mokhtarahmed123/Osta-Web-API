
using AutoMapper;
using Moq;
using Osta.Core.Feature.Technician.Query.Handler.TechnicianAvailabilitiesQueryHandler;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianAvailabilities;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianAvailabilities;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Test.TechnicianAvailabilitiesTesting.TechnicianAvailabilitiesCommandTesting
{
    public class GetTechnicianAvailabilityByIdQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<ITechnicianAvailabilityService> _availabilityServiceMock;

        private readonly GetTechnicianAvailabilityByIdQueryHandler _handler;

        public GetTechnicianAvailabilityByIdQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _availabilityServiceMock = new Mock<ITechnicianAvailabilityService>();

            _handler = new GetTechnicianAvailabilityByIdQueryHandler(
                _mapperMock.Object,
                _loggerMock.Object,
                _availabilityServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAvailabilityExists()
        {
            // Arrange
            var request = new GetTechnicianAvailabilityByIdQuery(1);

            var availability = new TechnicianAvailability
            {
                Id = 1,
                TechnicianId = "tech-123",
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsAvailable = true
            };

            var expectedResult = new GetTechnicianAvailabilityByIdResult
            {
                Id = 1,
                TechnicianId = "tech-123",
                Day = "Monday",
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                IsAvailable = true
            };

            _availabilityServiceMock
                .Setup(x => x.GetTechnicianAvailabilityAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(availability);

            _mapperMock
                .Setup(x => x.Map<GetTechnicianAvailabilityByIdResult>(
                    availability))
                .Returns(expectedResult);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);

            Assert.NotNull(result.Data);

            Assert.Equal(
                expectedResult,
                result.Data);

            _availabilityServiceMock.Verify(
                x => x.GetTechnicianAvailabilityAsync(
                    1,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<GetTechnicianAvailabilityByIdResult>(
                    availability),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenAvailabilityDoesNotExist()
        {
            // Arrange
            var request = new GetTechnicianAvailabilityByIdQuery(1);

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

            _availabilityServiceMock.Verify(
                x => x.GetTechnicianAvailabilityAsync(
                    1,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<GetTechnicianAvailabilityByIdResult>(
                    It.IsAny<TechnicianAvailability>()),
                Times.Never);
        }
    }
}

