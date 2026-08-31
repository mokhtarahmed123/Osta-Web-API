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
    public class GetAllTechnicianAvailabilitiesByTechnicianIdQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerServiceMock;
        private readonly Mock<ITechnicianAvailabilityService> _availabilityServiceMock;

        private readonly GetAllTechnicianAvailabilitiesByTechnicianIdQueryHandler _handler;

        public GetAllTechnicianAvailabilitiesByTechnicianIdQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _loggerServiceMock = new Mock<ILoggerService>();
            _availabilityServiceMock = new Mock<ITechnicianAvailabilityService>();

            _handler = new GetAllTechnicianAvailabilitiesByTechnicianIdQueryHandler(
                _mapperMock.Object,
                _loggerServiceMock.Object,
                _availabilityServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenNoAvailabilitiesExist()
        {
            // Arrange
            var technicianId = "tech-123";

            var request =
                new GetAllTechnicianAvailabilitiesByTechnicianIdQuery(
                    technicianId);

            _availabilityServiceMock
                .Setup(x =>
                    x.GetAllTechnicianAvailabilitiesByTechnicianIdAsync(
                        technicianId, CancellationToken.None))
                .ReturnsAsync(new List<TechnicianAvailability>());

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(
                "No availabilities found.",
                result.Message);

            _availabilityServiceMock.Verify(
                x => x.GetAllTechnicianAvailabilitiesByTechnicianIdAsync(
                    technicianId, CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAvailabilitiesExist()
        {
            // Arrange
            var technicianId = "tech-123";

            var request =
                new GetAllTechnicianAvailabilitiesByTechnicianIdQuery(
                    technicianId);

            var availabilities = new List<TechnicianAvailability>
            {
                new TechnicianAvailability
                {
                    Id = 1,
                    TechnicianId = technicianId,
                    DayOfWeek = DayOfWeek.Monday,
                    IsAvailable = true
                },
                new TechnicianAvailability
                {
                    Id = 2,
                    TechnicianId = technicianId,
                    DayOfWeek = DayOfWeek.Tuesday,
                    IsAvailable = true
                }
            };

            var mappedResult =
                new List<GetAllTechnicianAvailabilitiesByTechnicianIdResult>
                {
                    new GetAllTechnicianAvailabilitiesByTechnicianIdResult(),
                    new GetAllTechnicianAvailabilitiesByTechnicianIdResult()
                };

            _availabilityServiceMock
                .Setup(x =>
                    x.GetAllTechnicianAvailabilitiesByTechnicianIdAsync(
                        technicianId, CancellationToken.None))
                .ReturnsAsync(availabilities);

            _mapperMock
                .Setup(x =>
                    x.Map<List<GetAllTechnicianAvailabilitiesByTechnicianIdResult>>(
                        availabilities))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            //Assert.Equal("Count = 2", result.Message);

            _availabilityServiceMock.Verify(
                x => x.GetAllTechnicianAvailabilitiesByTechnicianIdAsync(
                    technicianId, CancellationToken.None),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetAllTechnicianAvailabilitiesByTechnicianIdResult>>(
                    availabilities),
                Times.Once);
        }
    }
}