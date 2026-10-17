
using Moq;
using Osta.Core.Feature.Authorization.Command.Handler;
using Osta.Core.Feature.Authorization.Command.Model.PermissionModel;
using Osta.Identity.Authorization;

namespace Osta.Test.AuthorizationTesting.AuthorizationCommandTesting
{
    public class RemovePermissionFromRoleCommandHandlerTests
    {

        private readonly Mock<IAuthorizationService> _authorizationServiceMock;

        private readonly RemovePermissionFromRoleCommandHandler _handler;

        public RemovePermissionFromRoleCommandHandlerTests()
        {


            _authorizationServiceMock = new Mock<IAuthorizationService>();

            _handler = new RemovePermissionFromRoleCommandHandler(

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
