using AutoMapper;
using Moq;
using Osta.Core.Feature.Technician.Command.Handler.TechnicianCommandHandler;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Test.TechnicianTesting.Command
{
    public class DeleteTechnicianCommandTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITechnicianService> _technicianServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<ITechnicianServiceService> _technicianServiceServiceMock;
        private readonly Mock<ITechnicianServiceAreasService> _technicianServiceAreasServiceMock;
        private readonly Mock<ITechnicianImagesService> _technicianImageServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly DeleteTechnicianCommandHandler _handler;

        public DeleteTechnicianCommandTest()
        {
            _mapperMock = new Mock<IMapper>();
            _technicianServiceMock = new Mock<ITechnicianService>();
            _loggerMock = new Mock<ILoggerService>();
            _technicianServiceServiceMock = new Mock<ITechnicianServiceService>();
            _technicianServiceAreasServiceMock = new Mock<ITechnicianServiceAreasService>();
            _technicianImageServiceMock = new Mock<ITechnicianImagesService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new DeleteTechnicianCommandHandler(
                _mapperMock.Object,
                _technicianServiceMock.Object,
                _loggerMock.Object,
                _technicianServiceServiceMock.Object,
                _technicianServiceAreasServiceMock.Object,
                _technicianImageServiceMock.Object,
                _currentUserServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnDeleted_WhenTechnicianDeletedSuccessfully()
        {
            // Arrange
            var command = new DeleteTechnicianCommand("tech1");

            var technician = new Technicians
            {
                Id = "tech1"
            };

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(command.technicianId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(technician);

            _technicianServiceServiceMock
                .Setup(x => x.DeleteAllService_technicianBy_technicianIdAsync(command.technicianId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _technicianImageServiceMock.Setup(x => x.Delete(command.technicianId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);


            _technicianServiceAreasServiceMock
                .Setup(x => x.DeleteAllTechnicianServiceAreasWithSpecifyTechnicianIdAsync(command.technicianId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _technicianServiceMock
                .Setup(x => x.DeleteTechnicianAsync(command.technicianId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);


            _technicianServiceServiceMock.Verify(
                x => x.DeleteAllService_technicianBy_technicianIdAsync(command.technicianId, It.IsAny<CancellationToken>()),
                Times.Once);

            _technicianServiceAreasServiceMock.Verify(
                x => x.DeleteAllTechnicianServiceAreasWithSpecifyTechnicianIdAsync(command.technicianId, It.IsAny<CancellationToken>()),
                Times.Once);

            _technicianServiceMock.Verify(
                x => x.DeleteTechnicianAsync(command.technicianId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenTechnicianDoesNotExist()
        {
            // Arrange
            var command = new DeleteTechnicianCommand("tech1");

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(command.technicianId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Technicians?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            //Assert.Equal("An error occurred while processing your request.", result.Message);

            _technicianServiceMock.Verify(
                x => x.DeleteTechnicianAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var command = new DeleteTechnicianCommand("tech1");

            var technician = new Technicians
            {
                Id = "tech1"
            };

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(command.technicianId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(technician);

            _technicianServiceServiceMock
                .Setup(x => x.DeleteAllService_technicianBy_technicianIdAsync(command.technicianId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("An error occurred while processing your request.", result.Message);
        }
    }
}