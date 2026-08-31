using AutoMapper;
using Moq;
using Osta.Core.Feature.Technician.Query.Handler.TechnicianQueryHandler;
using Osta.Core.Feature.Technician.Query.Model.TechnicianModel;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnician;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Test.TechnicianTesting.Query
{
    public class GetAllTechniciansWithRateQueryTest
    {
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILoggerService> _loggerMock = new();
        private readonly Mock<ITechnicianService> _technicianServiceMock = new();
        private readonly Mock<ITechnicianServiceService> _technicianServiceServiceMock = new();
        private readonly Mock<ITechnicianServiceAreasService> _technicianServiceAreasServiceMock = new();
        private readonly Mock<IServiceService> _serviceServiceMock = new();
        private readonly Mock<IServiceAreaService> _serviceAreaServiceMock = new();

        private readonly TechnicianQueryHandler _handler;

        public GetAllTechniciansWithRateQueryTest()
        {
            _handler = new TechnicianQueryHandler(
                _mapperMock.Object,
                _loggerMock.Object,
                _technicianServiceMock.Object,
                _technicianServiceServiceMock.Object,
                _technicianServiceAreasServiceMock.Object,
                _serviceServiceMock.Object,
                _serviceAreaServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnTechnicians_WhenMatchingRateExists()
        {
            // Arrange
            var query = new GetAllTechniciansWithRateQuery(4);

            var technicians = new List<Technicians>
            {
                new()
                {
                    Id = "tech1",
                    Rating = 4.8
                },
                new()
                {
                    Id = "tech2",
                    Rating = 4.2
                }
            };

            var expected = new List<GetAllTechniciansWithRateResult>
            {
                new(),
                new()
            };

            _technicianServiceMock
                .Setup(x => x.GetTechniciansByMinimumRateAsync(
                    query.Rate,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(technicians);

            _mapperMock
                .Setup(x => x.Map<List<GetAllTechniciansWithRateResult>>(technicians))
                .Returns(expected);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);

            _technicianServiceMock.Verify(x =>
                x.GetTechniciansByMinimumRateAsync(
                    query.Rate,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoTechnicianMatchesRate()
        {
            // Arrange
            var query = new GetAllTechniciansWithRateQuery(5);

            var technicians = new List<Technicians>();

            _technicianServiceMock
                .Setup(x => x.GetTechniciansByMinimumRateAsync(
                    query.Rate,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(technicians);

            _mapperMock
                .Setup(x => x.Map<List<GetAllTechniciansWithRateResult>>(technicians))
                .Returns(new List<GetAllTechniciansWithRateResult>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);

            _technicianServiceMock.Verify(x =>
                x.GetTechniciansByMinimumRateAsync(
                    query.Rate,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCallMapperOnce()
        {
            // Arrange
            var query = new GetAllTechniciansWithRateQuery(3);

            var technicians = new List<Technicians>
            {
                new()
            };

            _technicianServiceMock
                .Setup(x => x.GetTechniciansByMinimumRateAsync(
                    query.Rate,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(technicians);

            _mapperMock
                .Setup(x => x.Map<List<GetAllTechniciansWithRateResult>>(technicians))
                .Returns(new List<GetAllTechniciansWithRateResult>
                {
                    new()
                });

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mapperMock.Verify(x =>
                x.Map<List<GetAllTechniciansWithRateResult>>(technicians),
                Times.Once);
        }
    }
}