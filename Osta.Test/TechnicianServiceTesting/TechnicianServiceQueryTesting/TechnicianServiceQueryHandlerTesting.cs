
using AutoMapper;
using Moq;
using Osta.Core.Feature.Technician.Query.Handler.TechnicianServiceQueryHandler;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianService;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianService;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Test.TechnicianServiceTesting.TechnicianServiceQueryTesting
{
    public class TechnicianServiceQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerServiceMock;
        private readonly Mock<ITechnicianService> _technicianServiceMock;
        private readonly Mock<ITechnicianServiceService> _technicianServiceServiceMock;
        private readonly Mock<ITechnicianServiceAreasService> _technicianServiceAreasServiceMock;
        private readonly Mock<IServiceService> _serviceServiceMock;

        private readonly TechnicianServiceQueryHandler _handler;

        public TechnicianServiceQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _loggerServiceMock = new Mock<ILoggerService>();
            _technicianServiceMock = new Mock<ITechnicianService>();
            _technicianServiceServiceMock = new Mock<ITechnicianServiceService>();
            _technicianServiceAreasServiceMock = new Mock<ITechnicianServiceAreasService>();
            _serviceServiceMock = new Mock<IServiceService>();

            _handler = new TechnicianServiceQueryHandler(
                _mapperMock.Object,
                _loggerServiceMock.Object,
                _technicianServiceMock.Object,
                _technicianServiceServiceMock.Object,
                _technicianServiceAreasServiceMock.Object,
                _serviceServiceMock.Object);
        }

        [Fact]
        public async Task Handle_NoTechniciansFound_ShouldReturnNotFound()
        {
            // Arrange
            const int serviceId = 1;

            var request = new GetAllTechniciansWithServiceIdQuery(serviceId);

            _technicianServiceMock
                .Setup(x => x.GetTechniciansByServiceIdAsync(serviceId, CancellationToken.None))
                .ReturnsAsync(new List<Technicians>());

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Succeeded);
            Assert.Equal(
                "No technicians found.",
                response.Message);

            _serviceServiceMock.Verify(
                x => x.GetServiceAsync(It.IsAny<int>(), CancellationToken.None),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map<List<GetAllTechniciansWithServiceIdResult>>(
                    It.IsAny<object>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_TechniciansFound_ShouldReturnSuccess()
        {
            // Arrange
            const int serviceId = 1;

            var request = new GetAllTechniciansWithServiceIdQuery(serviceId);

            var technicians = new List<Technicians>
            {
                new Technicians(),
                new Technicians()
            };

            var service = new Data.Entities.Services.Service
            {
                Name = "Electrical Repair",
                Price = 150
            };

            var mappedResult = new List<GetAllTechniciansWithServiceIdResult>
            {
                new GetAllTechniciansWithServiceIdResult(),
                new GetAllTechniciansWithServiceIdResult()
            };

            _technicianServiceMock
                .Setup(x => x.GetTechniciansByServiceIdAsync(serviceId, CancellationToken.None))
                .ReturnsAsync(technicians);

            _serviceServiceMock
                .Setup(x => x.GetServiceAsync(serviceId, CancellationToken.None))
                .ReturnsAsync(service);

            _mapperMock
                .Setup(x =>
                    x.Map<List<GetAllTechniciansWithServiceIdResult>>(
                        technicians))
                .Returns(mappedResult);

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Succeeded);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.Data.Count);

            Assert.All(
                response.Data,
                item =>
                {
                    Assert.Equal(
                        "Electrical Repair",
                        item.ServiceName);

                    Assert.Equal(
                        150,
                        item.Price);
                });

            _technicianServiceMock.Verify(
                x => x.GetTechniciansByServiceIdAsync(serviceId, CancellationToken.None),
                Times.Once);

            _serviceServiceMock.Verify(
                x => x.GetServiceAsync(serviceId, CancellationToken.None),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetAllTechniciansWithServiceIdResult>>(
                    technicians),
                Times.Once);
        }

        [Fact]
        public async Task Handle_OneTechnicianFound_ShouldSetServiceNameAndPrice()
        {
            // Arrange
            const int serviceId = 5;

            var request = new GetAllTechniciansWithServiceIdQuery(serviceId);

            var technicians = new List<Technicians>
            {
                new Technicians()
            };

            var service = new Data.Entities.Services.Service
            {
                Name = "Plumbing",
                Price = 200
            };

            var mappedResult = new List<GetAllTechniciansWithServiceIdResult>
            {
                new GetAllTechniciansWithServiceIdResult()
            };

            _technicianServiceMock
                .Setup(x => x.GetTechniciansByServiceIdAsync(serviceId, CancellationToken.None))
                .ReturnsAsync(technicians);

            _serviceServiceMock
                .Setup(x => x.GetServiceAsync(serviceId, CancellationToken.None))
                .ReturnsAsync(service);

            _mapperMock
                .Setup(x =>
                    x.Map<List<GetAllTechniciansWithServiceIdResult>>(
                        technicians))
                .Returns(mappedResult);

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.True(response.Succeeded);
            Assert.Single(response.Data);

            Assert.Equal(
                "Plumbing",
                response.Data[0].ServiceName);

            Assert.Equal(
                200,
                response.Data[0].Price);
        }

        [Fact]
        public async Task Handle_MultipleTechniciansFound_ShouldSetSameServiceDataForAll()
        {
            // Arrange
            const int serviceId = 10;

            var request = new GetAllTechniciansWithServiceIdQuery(serviceId);

            var technicians = new List<Technicians>
            {
                new Technicians(),
                new Technicians(),
                new Technicians()
            };

            var service = new Data.Entities.Services.Service

            {
                Name = "Air Conditioning",
                Price = 350
            };

            var mappedResult = new List<GetAllTechniciansWithServiceIdResult>
            {
                new GetAllTechniciansWithServiceIdResult(),
                new GetAllTechniciansWithServiceIdResult(),
                new GetAllTechniciansWithServiceIdResult()
            };

            _technicianServiceMock
                .Setup(x => x.GetTechniciansByServiceIdAsync(serviceId, CancellationToken.None))
                .ReturnsAsync(technicians);

            _serviceServiceMock
                .Setup(x => x.GetServiceAsync(serviceId, CancellationToken.None))
                .ReturnsAsync(service);

            _mapperMock
                .Setup(x =>
                    x.Map<List<GetAllTechniciansWithServiceIdResult>>(
                        technicians))
                .Returns(mappedResult);

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.True(response.Succeeded);
            Assert.Equal(3, response.Data.Count);

            foreach (var technician in response.Data)
            {
                Assert.Equal(
                    "Air Conditioning",
                    technician.ServiceName);

                Assert.Equal(
                    350,
                    technician.Price);
            }
        }

        [Fact]
        public async Task Handle_ShouldNotCallServiceWhenNoTechniciansFound()
        {
            // Arrange
            const int serviceId = 99;

            var request = new GetAllTechniciansWithServiceIdQuery(serviceId);

            _technicianServiceMock
                .Setup(x => x.GetTechniciansByServiceIdAsync(serviceId, CancellationToken.None))
                .ReturnsAsync(new List<Technicians>());

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _serviceServiceMock.Verify(
                x => x.GetServiceAsync(serviceId, CancellationToken.None),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map<List<GetAllTechniciansWithServiceIdResult>>(
                    It.IsAny<List<Technicians>>()),
                Times.Never);
        }
    }
}

