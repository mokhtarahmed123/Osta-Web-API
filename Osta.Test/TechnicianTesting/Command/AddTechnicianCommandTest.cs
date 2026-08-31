using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using Osta.Core.Feature.Technician.Command.Handler.TechnicianCommandHandler;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianImage;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Data.Entities.Technician;
using Osta.Domain.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.Service.Model;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;
using System.Text;

namespace Osta.Test.TechnicianTesting.Command
{
    public class AddTechnicianCommandTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITechnicianService> _technicianServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<ITechnicianServiceService> _technicianServiceServiceMock;
        private readonly Mock<ITechnicianServiceAreasService> _technicianServiceAreasServiceMock;
        private readonly Mock<ITechnicianImagesService> _technicianImagesMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly RequestTechnicianCommandHandler _handler;

        public AddTechnicianCommandTest()
        {
            _mapperMock = new Mock<IMapper>();
            _technicianServiceMock = new Mock<ITechnicianService>();
            _loggerMock = new Mock<ILoggerService>();
            _technicianServiceServiceMock = new Mock<ITechnicianServiceService>();
            _technicianServiceAreasServiceMock = new Mock<ITechnicianServiceAreasService>();
            _technicianImagesMock = new Mock<ITechnicianImagesService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new AddTechnicianCommandHandler(
                _mapperMock.Object,
                _technicianServiceMock.Object,
                _loggerMock.Object,
                _technicianServiceServiceMock.Object,
                _technicianServiceAreasServiceMock.Object, _technicianImagesMock.Object, _currentUserServiceMock.Object);
        }

        // Remove all usage of 'command.Id' since 'AddTechnicianCommand' does not have an 'Id' property.
        // Instead, use a generated technician id string for test purposes.

        [Fact]
        public async Task Handle_ShouldReturnCreated_WhenTechnicianAddedSuccessfully()
        {
            // Arrange
            var technicianId = "tech1"; // Use a local variable for the technician id
            var command = new AddTechnicianCommand
            {
                Bio = "Backend Developer",
                YearsOfExperience = 5,
                NationalId = "45132098651320",
                ServiceAreas = new List<int> { 1, 2 },
                Images = new AddModelTechnicianImage
                {
                    ProfileImage = CreateFormFile("Image.png"),
                    FrontNationalIdImage = CreateFormFile("FrontId.png"),
                    BackNationalIdImage = CreateFormFile("BackId.png")
                }
            };

            var technician = new Technicians
            {
                //Id = technicianId, // If needed, set Id here
                Bio = command.Bio,
                YearsOfExperience = command.YearsOfExperience,
                NationalId = command.NationalId,
            };

            var technicianImages = new TechnicianImages
            {
                //TechnicianId = technicianId,
                ProfilePicture = "Image.png",
                FrontNationalIdImage = "FrontId.png",
                BackNationalIdImage = "BackId.png"
            };
            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Technicians?)null);

            _mapperMock
                .Setup(x => x.Map<Technicians>(command))
                .Returns(technician);

            _technicianServiceMock
                .Setup(x => x.AddTechnicianAsync(
                    technician,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _technicianImagesMock
                .Setup(x => x.Add(
                    technicianId,
                    It.IsAny<TechnicianImageModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(technicianImages);

            _technicianServiceAreasServiceMock
                .Setup(x => x.AddTechnicianServiceAreasRangeAsync(
                    It.IsAny<ICollection<TechnicianServiceArea>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);

            Assert.Equal(
                "Technician created successfully.",
                result.Data);

            _technicianServiceMock.Verify(x =>
                x.AddTechnicianAsync(
                    technician,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _technicianServiceAreasServiceMock.Verify(x =>
                x.AddTechnicianServiceAreasRangeAsync(
                    It.Is<ICollection<TechnicianServiceArea>>(
                        l => l.Count == 2), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var technicianId = "tech1";
            var command = new AddTechnicianCommand
            {
                Bio = "Backend",
                YearsOfExperience = 5,
                NationalId = "45132098651320",
                ServiceAreas = new List<int> { 1 }
            };

            var technician = new Technicians
            {
                //Id = technicianId
            };

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(technicianId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Technicians?)null);

            _mapperMock
                .Setup(x => x.Map<Technicians>(command))
                .Returns(technician);

            _technicianServiceMock
                .Setup(x => x.AddTechnicianAsync(
                    It.IsAny<Technicians>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("An error occurred while processing your request.", result.Message);

            _technicianServiceMock.Verify(x =>
                x.AddTechnicianAsync(
                    It.IsAny<Technicians>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        private static IFormFile CreateFormFile(string fileName)
        {
            var bytes = Encoding.UTF8.GetBytes("fake image content");

            var stream = new MemoryStream(bytes);

            return new FormFile(
                stream,
                0,
                bytes.Length,
                "file",
                fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };
        }
    }
}