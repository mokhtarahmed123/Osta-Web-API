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
    public class GetAllTechniciansQueryTest
    {
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILoggerService> _loggerMock = new();
        private readonly Mock<ITechnicianService> _technicianServiceMock = new();
        private readonly Mock<ITechnicianServiceService> _technicianServiceServiceMock = new();
        private readonly Mock<ITechnicianServiceAreasService> _technicianServiceAreasServiceMock = new();
        private readonly Mock<IServiceService> _serviceServiceMock = new();
        private readonly Mock<IServiceAreaService> _serviceAreaServiceMock = new();

        private readonly TechnicianQueryHandler _handler;

        public GetAllTechniciansQueryTest()
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
        public async Task Handle_ShouldReturnAllTechnicians()
        {
            // Arrange
            var technicians = new List<Technicians>
            {
                new(){ Id="1", Bio="Backend"},
                new(){ Id="2", Bio="Frontend"}
            };

            var services = new List<TechnicianService>();

            var serviceAreas = new List<TechnicianServiceArea>();

            var resultList = new List<GetAllTechniciansResult>
            {
                new(){ Id="1"},
                new(){ Id="2"}
            };

            _technicianServiceMock
                .Setup(x => x.GetAllTechniciansAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(technicians);

            _technicianServiceServiceMock
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(services);

            _technicianServiceAreasServiceMock
                .Setup(x => x.GetAllTechnicianServiceAreasAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceAreas);

            _mapperMock
                .Setup(x => x.Map<List<GetAllTechniciansResult>>(technicians))
                .Returns(resultList);

            // Act
            var result = await _handler.Handle(new GetAllTechniciansQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoTechniciansExist()
        {
            // Arrange
            var technicians = new List<Technicians>();

            _technicianServiceMock
                .Setup(x => x.GetAllTechniciansAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(technicians);

            _technicianServiceServiceMock
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TechnicianService>());

            _technicianServiceAreasServiceMock
                .Setup(x => x.GetAllTechnicianServiceAreasAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TechnicianServiceArea>());

            _mapperMock
                .Setup(x => x.Map<List<GetAllTechniciansResult>>(technicians))
                .Returns(new List<GetAllTechniciansResult>());

            // Act
            var result = await _handler.Handle(new GetAllTechniciansQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Empty(result.Data);
        }
    }
}