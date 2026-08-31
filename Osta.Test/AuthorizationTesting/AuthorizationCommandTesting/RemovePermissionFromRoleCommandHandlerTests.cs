
using Moq;
using Osta.Core.Feature.Authorization.Command.Handler;
using Osta.Core.Feature.Authorization.Command.Model.PermissionModel;
using Osta.Identity.Authorization;

namespace Osta.Test.AuthorizationTesting.AuthorizationCommandTesting
{
    public class RemovePermissionFromRoleCommandHandlerTests
    {
        private readonly Mock<Microsoft.AspNetCore.Identity.UserManager<Data.Entities.Identity.User>> _userManagerMock;
        private readonly Mock<Microsoft.AspNetCore.Identity.RoleManager<Data.Entities.Identity.Role>> _roleManagerMock;
        private readonly Mock<IAuthorizationService> _authorizationServiceMock;

        private readonly RemovePermissionFromRoleCommandHandler _handler;

        public RemovePermissionFromRoleCommandHandlerTests()
        {
            _userManagerMock = new Mock<Microsoft.AspNetCore.Identity.UserManager<Data.Entities.Identity.User>>(
                Mock.Of<Microsoft.AspNetCore.Identity.IUserStore<Data.Entities.Identity.User>>(),
                null!, null!, null!, null!, null!, null!, null!, null!);

            _roleManagerMock = new Mock<Microsoft.AspNetCore.Identity.RoleManager<Data.Entities.Identity.Role>>(
                Mock.Of<Microsoft.AspNetCore.Identity.IRoleStore<Data.Entities.Identity.Role>>(),
                null!, null!, null!, null!);

            _authorizationServiceMock = new Mock<IAuthorizationService>();

            _handler = new RemovePermissionFromRoleCommandHandler(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _authorizationServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenPermissionIsRemovedSuccessfully()
        {
            // Arrange
            var roleId = "role-123";
            var permissionId = "permission-123";

            var request = new RemovePermissionFromRoleCommand(
                roleId,
                permissionId);

            _authorizationServiceMock
                .Setup(x => x.RemovePermissionFromRoleAsync(
                    permissionId,
                    roleId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "Permission removed from role successfully.",
                result.Data);

            _authorizationServiceMock.Verify(
                x => x.RemovePermissionFromRoleAsync(
                    permissionId,
                    roleId),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenPermissionOrRoleDoesNotExist()
        {
            // Arrange
            var roleId = "role-123";
            var permissionId = "permission-123";

            var request = new RemovePermissionFromRoleCommand(
                roleId,
                permissionId);

            _authorizationServiceMock
                .Setup(x => x.RemovePermissionFromRoleAsync(
                    permissionId,
                    roleId))
                .ThrowsAsync(
                    new KeyNotFoundException("Permission or role not found."));

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "Permission or role not found.",
                result.Message);

            Assert.Null(result.Data);

            _authorizationServiceMock.Verify(
                x => x.RemovePermissionFromRoleAsync(
                    permissionId,
                    roleId),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectPermissionAndRoleIds()
        {
            // Arrange
            var roleId = "role-456";
            var permissionId = "permission-789";

            var request = new RemovePermissionFromRoleCommand(
                roleId,
                permissionId);

            _authorizationServiceMock
                .Setup(x => x.RemovePermissionFromRoleAsync(
                    permissionId,
                    roleId))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _authorizationServiceMock.Verify(
                x => x.RemovePermissionFromRoleAsync(
                    permissionId,
                    roleId),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPropagateUnexpectedException()
        {
            // Arrange
            var roleId = "role-123";
            var permissionId = "permission-123";

            var request = new RemovePermissionFromRoleCommand(
                roleId,
                permissionId);

            _authorizationServiceMock
                .Setup(x => x.RemovePermissionFromRoleAsync(
                    permissionId,
                    roleId))
                .ThrowsAsync(
                    new InvalidOperationException("Unexpected error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(
                    request,
                    CancellationToken.None));

            Assert.Equal(
                "Unexpected error",
                exception.Message);

            _authorizationServiceMock.Verify(
                x => x.RemovePermissionFromRoleAsync(
                    permissionId,
                    roleId),
                Times.Once);
        }
    }
}
