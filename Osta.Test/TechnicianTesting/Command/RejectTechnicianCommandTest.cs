using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Core.Feature.Technician.Command.Handler.TechnicianCommandHandler;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Data.Entities.Identity;
using Osta.Data.Entities.Technician;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;
using System.Net;

namespace Osta.Test.TechnicianTesting.Command
{
    public class RejectTechnicianCommandTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITechnicianService> _technicianServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<ITechnicianServiceService> _technicianServiceServiceMock;
        private readonly Mock<ITechnicianServiceAreasService> _technicianServiceAreasServiceMock;
        private readonly Mock<ITechnicianImagesService> _technicianImagesServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ISendNotificationMessage> _sendNotificationMessageMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly RejectTechnicianCommandHandler _handler;

        public RejectTechnicianCommandTest()
        {
            _mapperMock = new Mock<IMapper>();
            _technicianServiceMock = new Mock<ITechnicianService>();
            _loggerMock = new Mock<ILoggerService>();
            _technicianServiceServiceMock = new Mock<ITechnicianServiceService>();
            _technicianServiceAreasServiceMock = new Mock<ITechnicianServiceAreasService>();
            _technicianImagesServiceMock = new Mock<ITechnicianImagesService>();
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            _sendNotificationMessageMock = new Mock<ISendNotificationMessage>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new RejectTechnicianCommandHandler(
                _mapperMock.Object,
                _technicianServiceMock.Object,
                _loggerMock.Object,
                _technicianServiceServiceMock.Object,
                _technicianServiceAreasServiceMock.Object,
                _technicianImagesServiceMock.Object,
                _userManagerMock.Object,
                _sendNotificationMessageMock.Object,
                _currentUserServiceMock.Object
            );
        }
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenTechnicianRejected()
        {
            // Arrange
            var command = new RejectTechnicianCommand("tech1")
            {
                Reason = "Incomplete documents"
            };

            var technician = new Technicians
            {
                Id = "tech1"
            };

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(
                    command.TechId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(technician);

            _technicianServiceMock
                .Setup(x => x.RejectRequestOfTechnicianAsync(
                    command.TechId,
                    command.Reason, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // UserManager
            var user = new User
            {
                Id = command.TechId,
                Email = "tech@test.com"
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync(command.TechId))
                .ReturnsAsync(user);

            // Notification
            _sendNotificationMessageMock
                .Setup(x => x.SendNotification(
                    It.IsAny<TechnicianStatusNotificationMessage>(),
                    "technician-request"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            _technicianServiceMock.Verify(x =>
                x.GetTechnicianAsync(
                    command.TechId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _technicianServiceMock.Verify(x =>
                x.RejectRequestOfTechnicianAsync(
                    command.TechId,
                    command.Reason, CancellationToken.None),
                Times.Once);

            _userManagerMock.Verify(x =>
                x.FindByIdAsync(command.TechId),
                Times.Once);

            _sendNotificationMessageMock.Verify(x =>
                x.SendNotification(
                    It.Is<TechnicianStatusNotificationMessage>(n =>
                        n.Id == command.TechId &&
                        n.Email == "tech@test.com" &&
                        n.ReasonOfReject == command.Reason &&
                        n.StatusOfRequest == "Rejected"),
                    "technician-request"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenTechnicianDoesNotExist()
        {
            // Arrange
            var command = new RejectTechnicianCommand("tech1")
            {
                Reason = "Rejected"
            };

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(command.TechId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Technicians?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Technician not found.", result.Message);

            _technicianServiceMock.Verify(x =>
                x.RejectRequestOfTechnicianAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenKeyNotFoundExceptionOccurs()
        {
            // Arrange
            var command = new RejectTechnicianCommand("tech1")
            {
                Reason = "Rejected"
            };

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(command.TechId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Technician not found.", result.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var command = new RejectTechnicianCommand("tech1")
            {
                Reason = "Rejected"
            };

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(command.TechId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database Error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("An error occurred while processing your request.", result.Message);
        }
    }
}