using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Core.Feature.Technician.Command.Handler.TechnicianCommandHandler;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Data.Entities.Identity;
using Osta.Data.Entities.Technician;
using Osta.Domain.Entities.Technician;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;
using System.Net;

namespace Osta.Test.TechnicianTesting.Command
{
    public class VerifyTechnicianCommandTest
    {

        private readonly Mock<ITechnicianService> _technicianServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly Mock<ISendNotificationMessage> _sendNotificationMessageMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ITechnicianWalletService> _technicianWalletServiceMock;

        private readonly VerifyTechnicianCommandHandler _handler;

        public VerifyTechnicianCommandTest()
        {
            _technicianServiceMock = new Mock<ITechnicianService>();
            _loggerMock = new Mock<ILoggerService>();
            _sendNotificationMessageMock = new Mock<ISendNotificationMessage>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _technicianWalletServiceMock = new Mock<ITechnicianWalletService>();

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

            _handler = new VerifyTechnicianCommandHandler(

                _technicianServiceMock.Object,
                _loggerMock.Object,

                _sendNotificationMessageMock.Object,
                _userManagerMock.Object,

                _technicianWalletServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenTechnicianVerified()
        {
            // Arrange
            var command = new VerifyTechnicianCommand("tech1");

            var technician = new Technicians
            {
                Id = "tech1"
            };

            var user = new User
            {
                Id = "tech1",
                Email = "tech1@test.com"
            };

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(
                    command.TechId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(technician);

            _technicianServiceMock
                .Setup(x => x.VerifyRequestOfTechnicianAsync(
                    command.TechId, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // UserManager
            _userManagerMock
                .Setup(x => x.FindByIdAsync(command.TechId))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.AddToRoleAsync(
                    user,
                    "Technicians"))
                .ReturnsAsync(IdentityResult.Success);

            // Notification
            _sendNotificationMessageMock
                .Setup(x => x.SendNotification(
                    It.IsAny<TechnicianStatusNotificationMessage>(),
                    "technician-request"))
                .Returns(Task.CompletedTask);

            // Wallet does not exist
            _technicianWalletServiceMock
                .Setup(x => x.GetWalletAsync(command.TechId, CancellationToken.None))
                .ReturnsAsync((TechnicianWallet)null);

            // Create wallet
            _technicianWalletServiceMock
                .Setup(x => x.CreateWalletAsync(
                    It.IsAny<TechnicianWallet>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((TechnicianWallet)null);

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
                x.VerifyRequestOfTechnicianAsync(
                    command.TechId, CancellationToken.None),
                Times.Once);

            _userManagerMock.Verify(x =>
                x.FindByIdAsync(command.TechId),
                Times.Once);

            _userManagerMock.Verify(x =>
                x.AddToRoleAsync(
                    user,
                    "Technicians"),
                Times.Once);

            _sendNotificationMessageMock.Verify(x =>
                x.SendNotification(
                    It.IsAny<TechnicianStatusNotificationMessage>(),
                    "technician-request"),
                Times.Once);

            _technicianWalletServiceMock.Verify(x =>
                x.GetWalletAsync(command.TechId, CancellationToken.None),
                Times.Once);

            _technicianWalletServiceMock.Verify(x =>
                x.CreateWalletAsync(
                    It.Is<TechnicianWallet>(w =>
                        w.TechnicianId == command.TechId &&
                        w.Amount == 0),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenTechnicianDoesNotExist()
        {
            // Arrange
            var command = new VerifyTechnicianCommand("tech1");

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(command.TechId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Technicians?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Technician not found.", result.Message);

            _technicianServiceMock.Verify(x =>
                x.VerifyRequestOfTechnicianAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenKeyNotFoundExceptionOccurs()
        {
            // Arrange
            var command = new VerifyTechnicianCommand("tech1");

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
            var command = new VerifyTechnicianCommand("tech1");

            _technicianServiceMock
                .Setup(x => x.GetTechnicianAsync(command.TechId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("An error occurred while processing your request.", result.Message);
        }
    }
}