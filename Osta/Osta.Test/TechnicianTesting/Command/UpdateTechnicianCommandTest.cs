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
    public class UpdateTechnicianCommandTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITechnicianService> _technicianServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly Mock<ITechnicianServiceAreasService> _technicianServiceAreasServiceMock;
        private readonly Mock<ITechnicianImagesService> _technicianImagesServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly UpdateTechnicianCommandHandler _handler;

        public UpdateTechnicianCommandTest()
        {
            _mapperMock = new Mock<IMapper>();
            _technicianServiceMock = new Mock<ITechnicianService>();
            _loggerMock = new Mock<ILoggerService>();

            _technicianServiceAreasServiceMock = new Mock<ITechnicianServiceAreasService>();
            _technicianImagesServiceMock = new Mock<ITechnicianImagesService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            // مهم جداً
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("tech1");

            _handler = new UpdateTechnicianCommandHandler(
                _mapperMock.Object,
                _technicianServiceMock.Object,
                _loggerMock.Object,

                _technicianServiceAreasServiceMock.Object,
                _technicianImagesServiceMock.Object,
                _currentUserServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenTechnicianDoesNotExist()
        {
            // Arrange
            var command = new UpdateTechnicianCommand
            {
                Bio = "Backend Developer",
                YearsOfExperience = 5,
                NationalId = "45132098651320",
                ServiceAreas = new List<int> { 1, 2 }
            };

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Technicians?)null);

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Technician not found.", result.Message);

            _technicianServiceMock.Verify(x =>
                x.UpdateTechnicianAsync(
                    It.IsAny<string>(),
                    It.IsAny<Technicians>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUpdated_WhenTechnicianUpdatedSuccessfully()
        {
            // Arrange
            var command = new UpdateTechnicianCommand
            {
                Bio = "Updated Bio",
                YearsOfExperience = 8,
                NationalId = "45132098651320",
                ServiceAreas = new List<int> { 1, 2 }
            };

            var technician = new Technicians
            {
                Id = "tech1",
                Bio = "Old Bio",
                YearsOfExperience = 2
            };

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(technician);

            _mapperMock
                .Setup(x => x.Map(command, technician));

            _technicianServiceMock
                .Setup(x => x.UpdateTechnicianAsync(
                    "tech1",
                    technician,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _technicianServiceAreasServiceMock
                .Setup(x => x.GetTechnicianServiceAreasByTechnicianIdAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TechnicianServiceArea>());

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(
                "Technician updated successfully.",
                result.Message);

            _technicianServiceMock.Verify(x =>
                x.UpdateTechnicianAsync(
                    "tech1",
                    technician,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _technicianImagesServiceMock.Verify(
                x => x.Update(
                    It.IsAny<string>(),
                    It.IsAny<Service.Model.TechnicianImageModel>(), CancellationToken.None),
                Times.Never);

            _technicianServiceAreasServiceMock.Verify(
                x => x.DeleteAllTechnicianServiceAreasWithSpecifyTechnicianIdAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldDeleteOldAreasAndAddNewAreas_WhenServiceAreasExist()
        {
            // Arrange
            var command = new UpdateTechnicianCommand
            {
                Bio = "Updated",
                YearsOfExperience = 10,
                ServiceAreas = new List<int> { 5, 6 }
            };

            var technician = new Technicians
            {
                Id = "tech1"
            };

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(technician);

            _mapperMock
                .Setup(x => x.Map(command, technician));

            _technicianServiceMock
                .Setup(x => x.UpdateTechnicianAsync(
                    "tech1",
                    technician,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _technicianServiceAreasServiceMock
                .Setup(x => x.GetTechnicianServiceAreasByTechnicianIdAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TechnicianServiceArea>
                {
                    new TechnicianServiceArea()
                });

            _technicianServiceAreasServiceMock
                .Setup(x => x.DeleteAllTechnicianServiceAreasWithSpecifyTechnicianIdAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _technicianServiceAreasServiceMock
                .Setup(x => x.AddTechnicianServiceAreasRangeAsync(
                    It.IsAny<ICollection<TechnicianServiceArea>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(
                "Technician updated successfully.",
                result.Message);

            _technicianServiceAreasServiceMock.Verify(
                x => x.DeleteAllTechnicianServiceAreasWithSpecifyTechnicianIdAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _technicianServiceAreasServiceMock.Verify(
                x => x.AddTechnicianServiceAreasRangeAsync(
                    It.Is<ICollection<TechnicianServiceArea>>(
                        list =>
                            list.Count == 2 &&
                            list.All(x => x.TechnicianId == "tech1")),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var command = new UpdateTechnicianCommand();

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(
                    "tech1",
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);

            Assert.Equal(
                "An error occurred while processing your request.",
                result.Message);
        }
    }
}