using AutoMapper;
using Moq;
using Osta.Core.Feature.Roles.Command.Handler;
using Osta.Core.Feature.Roles.Command.Model;
using Osta.Data.Entities.Identity;
using Osta.Identity.Roles;
using Osta.SharedKernel.Logging;
using System.Net;

namespace Osta.Test.RolesTesting.RoleCommandTesting
{
    public class DeleteRoleCommandTesting
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IRoleService> roleServiceMock;
        private readonly Mock<ILoggerService> loggerServiceMock;

        private readonly DeleteRoleCommandHandler handler;

        public DeleteRoleCommandTesting()
        {
            mapperMock = new Mock<IMapper>();
            roleServiceMock = new Mock<IRoleService>();
            loggerServiceMock = new Mock<ILoggerService>();

            handler = new DeleteRoleCommandHandler(
                mapperMock.Object,
                roleServiceMock.Object,
                loggerServiceMock.Object);
        }
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenRoleIsDeletedSuccessfully()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();

            var request = new DeleteRoleCommand(roleId)
            {
                RoleId = roleId
            };

            var role = new Role
            {
                Id = roleId
            };

            roleServiceMock
                .Setup(x => x.GetRoleByIdAsync(roleId))
                .ReturnsAsync(role);

            roleServiceMock
                .Setup(x => x.DeleteRoleAsync(roleId))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Role deleted successfully.", result.Data);

            roleServiceMock.Verify(
                x => x.GetRoleByIdAsync(roleId),
                Times.Once);

            roleServiceMock.Verify(
                x => x.DeleteRoleAsync(roleId),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();

            var request = new DeleteRoleCommand(roleId)
            {
                RoleId = roleId
            };

            roleServiceMock
                .Setup(x => x.GetRoleByIdAsync(roleId))
                .ReturnsAsync((Role?)null);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

            roleServiceMock.Verify(
                x => x.GetRoleByIdAsync(roleId),
                Times.Once);

            roleServiceMock.Verify(
                x => x.DeleteRoleAsync(It.IsAny<string>()),
                Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenRoleDeletionFails()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();

            var request = new DeleteRoleCommand(roleId)
            {
                RoleId = roleId
            };

            var role = new Role
            {
                Id = roleId
            };

            roleServiceMock
                .Setup(x => x.GetRoleByIdAsync(roleId))
                .ReturnsAsync(role);

            roleServiceMock
                .Setup(x => x.DeleteRoleAsync(roleId))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            roleServiceMock.Verify(
                x => x.GetRoleByIdAsync(roleId),
                Times.Once);

            roleServiceMock.Verify(
                x => x.DeleteRoleAsync(roleId),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();

            var request = new DeleteRoleCommand(roleId)
            {
                RoleId = roleId
            };

            var exception = new Exception("Database error");

            roleServiceMock
                .Setup(x => x.GetRoleByIdAsync(roleId))
                .ThrowsAsync(exception);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            loggerServiceMock.Verify(
                x => x.LogError(
                    exception,
                    It.IsAny<string>(),
                    It.IsAny<object[]>()),
                Times.Once);
        }
    }
}
