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
    public class UpdateRoleCommandTesting
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IRoleService> roleServiceMock;
        private readonly Mock<ILoggerService> loggerServiceMock;

        private readonly UpdateRoleCommandHandler handler;

        public UpdateRoleCommandTesting()
        {
            mapperMock = new Mock<IMapper>();
            roleServiceMock = new Mock<IRoleService>();
            loggerServiceMock = new Mock<ILoggerService>();

            handler = new UpdateRoleCommandHandler(
                mapperMock.Object,
                roleServiceMock.Object,
                loggerServiceMock.Object);
        }
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenRoleIsUpdatedSuccessfully()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();

            var request = new UpdateRoleCommand(roleId)
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

            mapperMock
                .Setup(x => x.Map(request, role));

            roleServiceMock
                .Setup(x => x.UpdateRoleAsync(roleId, role))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Role updated successfully.", result.Data);

            roleServiceMock.Verify(
                x => x.GetRoleByIdAsync(roleId),
                Times.Once);

            mapperMock.Verify(
                x => x.Map(request, role),
                Times.Once);

            roleServiceMock.Verify(
                x => x.UpdateRoleAsync(roleId, role),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();

            var request = new UpdateRoleCommand(roleId)
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

            mapperMock.Verify(
                x => x.Map(request, It.IsAny<Role>()),
                Times.Never);

            roleServiceMock.Verify(
                x => x.UpdateRoleAsync(
                    It.IsAny<string>(),
                    It.IsAny<Role>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenRoleUpdateFails()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();

            var request = new UpdateRoleCommand(roleId)
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

            mapperMock
                .Setup(x => x.Map(request, role));

            roleServiceMock
                .Setup(x => x.UpdateRoleAsync(roleId, role))
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

            mapperMock.Verify(
                x => x.Map(request, role),
                Times.Once);

            roleServiceMock.Verify(
                x => x.UpdateRoleAsync(roleId, role),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();

            var request = new UpdateRoleCommand(roleId)
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