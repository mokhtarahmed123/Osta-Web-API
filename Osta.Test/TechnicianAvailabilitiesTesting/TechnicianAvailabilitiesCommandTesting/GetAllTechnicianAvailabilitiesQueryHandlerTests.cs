
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
    public class GetAllTechnicianAvailabilitiesQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<ITechnicianAvailabilityService> _availabilityServiceMock;

        private readonly GetAllTechnicianAvailabilitiesQueryHandler _handler;

        public GetAllTechnicianAvailabilitiesQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _availabilityServiceMock =
                new Mock<ITechnicianAvailabilityService>();

            _handler = new GetAllTechnicianAvailabilitiesQueryHandler(
                _mapperMock.Object,
                _loggerMock.Object,
                _availabilityServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAvailabilitiesExist()
        {
            // Arrange
            var request = new GetAllTechnicianAvailabilitiesQuery();

            var availabilities = new List<TechnicianAvailability>
            {
                new TechnicianAvailability
                {
                    Id = 1,
                    TechnicianId = "tech-123",
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(17, 0),
                    IsAvailable = true
                },
                new TechnicianAvailability
                {
                    Id = 2,
                    TechnicianId = "tech-456",
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(18, 0),
                    IsAvailable = true
                }
            };

            var expectedResult =
                new List<GetAllTechnicianAvailabilitiesResult>
                {
                    new GetAllTechnicianAvailabilitiesResult
                    {
                        Id = 1,
                        TechnicianId = "tech-123",
                        Day = DayOfWeek.Monday.ToString(),
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(17, 0),
                        IsAvailable = true
                    },
                    new GetAllTechnicianAvailabilitiesResult
                    {
                        Id = 2,
                        TechnicianId = "tech-456",
                        Day = DayOfWeek.Tuesday.ToString(),
                        StartTime = new TimeOnly(10, 0),
                        EndTime = new TimeOnly(18, 0),
                        IsAvailable = true
                    }
                };

            _availabilityServiceMock
                .Setup(x => x.GetAllTechnicianAvailabilitiesAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(availabilities);

            _mapperMock
                .Setup(x => x.Map<List<GetAllTechnicianAvailabilitiesResult>>(
                    availabilities))
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
                expectedResult.Count,
                result.Data.Count);

            //Assert.Equal(
            //    "Count = 2",
            //    result.Message);

            _availabilityServiceMock.Verify(
                x => x.GetAllTechnicianAvailabilitiesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetAllTechnicianAvailabilitiesResult>>(
                    availabilities),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenNoAvailabilitiesExist()
        {
            // Arrange
            var request = new GetAllTechnicianAvailabilitiesQuery();

            var availabilities =
                new List<TechnicianAvailability>();

            _availabilityServiceMock
                .Setup(x => x.GetAllTechnicianAvailabilitiesAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(availabilities);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);

            Assert.Equal(
                "No technician availabilities found.",
                result.Message);

            _availabilityServiceMock.Verify(
                x => x.GetAllTechnicianAvailabilitiesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetAllTechnicianAvailabilitiesResult>>(
                    It.IsAny<List<TechnicianAvailability>>()),
                Times.Never);
        }
    }
}

