using AutoMapper;
using Moq;
using Osta.Core.Feature.Service.Query.Result;
using Osta.Core.Feature.ServiceArea.Query.Result;
using Osta.Core.Feature.Technician.Query.Handler.TechnicianQueryHandler;
using Osta.Core.Feature.Technician.Query.Model.TechnicianModel;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnician;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Test.TechnicianTesting.Query
{
    public class GetTechnicianByIdQueryTest
    {
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILoggerService> _loggerMock = new();
        private readonly Mock<ITechnicianService> _technicianServiceMock = new();
        private readonly Mock<ITechnicianServiceService> _technicianServiceServiceMock = new();
        private readonly Mock<ITechnicianServiceAreasService> _technicianServiceAreasServiceMock = new();
        private readonly Mock<IServiceService> _serviceServiceMock = new();
        private readonly Mock<IServiceAreaService> _serviceAreaServiceMock = new();

        private readonly TechnicianQueryHandler _handler;

        public GetTechnicianByIdQueryTest()
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
        public async Task Handle_ShouldReturnTechnician_WhenFound()
        {
            // Arrange
            var technician = new Technicians
            {
                Id = "tech1",
                Bio = "Backend Developer"
            };

            var technicianResult = new GetTechnicianByIdResult
            {
                Bio = "Backend Developer",
                Services = new(),
                Areas = new()
            };

            var services = new List<Data.Entities.Services.Service>();
            var serviceResults = new List<GetServiceByIdResult>();

            var areas = new List<ServiceArea>();
            var areaResults = new List<GetAllServiceAreasResult>();

            _technicianServiceMock
                .Setup(x => x.GetTechnicianWithServiceAndServiceAreaAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(technician);

            _mapperMock
                .Setup(x => x.Map<GetTechnicianByIdResult>(technician))
                .Returns(technicianResult);

            _serviceServiceMock
                .Setup(x => x.GetServicesByTechnicianIdAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(services);

            _serviceAreaServiceMock
                .Setup(x => x.GetServiceAreaWithSpecificTechIdAsync("tech1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(areas);

            _mapperMock
                .Setup(x => x.Map<List<GetServiceByIdResult>>(services))
                .Returns(serviceResults);

            _mapperMock
                .Setup(x => x.Map<List<GetAllServiceAreasResult>>(areas))
                .Returns(areaResults);

            // Act
            var result = await _handler.Handle(
                new GetTechnicianByIdQuery("tech1"),
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("Backend Developer", result.Data.Bio);

            _technicianServiceMock.Verify(x =>
                x.GetTechnicianWithServiceAndServiceAreaAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenTechnicianDoesNotExist()
        {
            // Arrange
            _technicianServiceMock
                .Setup(x => x.GetTechnicianWithServiceAndServiceAreaAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Technicians?)null);

            // Act
            var result = await _handler.Handle(
                new GetTechnicianByIdQuery("tech1"),
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Technician not found.", result.Message);
        }

        [Fact]
        public async Task Handle_ShouldMapServicesAndAreas()
        {
            // Arrange
            var technician = new Technicians
            {
                Id = "tech1"
            };

            var dto = new GetTechnicianByIdResult
            {
                Bio = "Backend",
                Services = new(),
                Areas = new()
            };

            var services = new List<Data.Entities.Services.Service>
            {
                new()
            };

            var serviceDtos = new List<GetServiceByIdResult>
            {
                new()
            };

            var areas = new List<ServiceArea>
            {
                new()
            };

            var areaDtos = new List<GetAllServiceAreasResult>
            {
                new()
            };

            _technicianServiceMock
                .Setup(x => x.GetTechnicianWithServiceAndServiceAreaAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(technician);

            _mapperMock
                .Setup(x => x.Map<GetTechnicianByIdResult>(technician))
                .Returns(dto);

            _serviceServiceMock
                .Setup(x => x.GetServicesByTechnicianIdAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(services);

            _serviceAreaServiceMock
                .Setup(x => x.GetServiceAreaWithSpecificTechIdAsync("tech1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(areas);

            _mapperMock
                .Setup(x => x.Map<List<GetServiceByIdResult>>(services))
                .Returns(serviceDtos);

            _mapperMock
                .Setup(x => x.Map<List<GetAllServiceAreasResult>>(areas))
                .Returns(areaDtos);

            // Act
            var result = await _handler.Handle(
                new GetTechnicianByIdQuery("tech1"),
                CancellationToken.None);

            // Assert
            Assert.Single(result.Data.Services);
            Assert.Single(result.Data.Areas);
        }
    }
}