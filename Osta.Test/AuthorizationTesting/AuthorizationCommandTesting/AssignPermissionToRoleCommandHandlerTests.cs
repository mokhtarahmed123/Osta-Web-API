
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Core.Feature.Authorization.Command.Handler;
using Osta.Core.Feature.Authorization.Command.Model.PermissionModel;
using Osta.Data.Entities.Identity;
using Osta.Identity.Authorization;

namespace Osta.Test.AuthorizationTesting.AuthorizationCommandTesting
{
    public class AssignPermissionToRoleCommandHandlerTests
    {

        private readonly Mock<RoleManager<Role>> _roleManagerMock;
        private readonly Mock<IAuthorizationService> _authorizationServiceMock;

        private readonly AssignPermissionToRoleCommandHandler _handler;

        public AssignPermissionToRoleCommandHandlerTests()
        {

            _roleManagerMock = new Mock<RoleManager<Role>>(
                Mock.Of<IRoleStore<Role>>(),
                null!, null!, null!, null!);

            _authorizationServiceMock = new Mock<IAuthorizationService>();

            _handler = new AssignPermissionToRoleCommandHandler(

                _roleManagerMock.Object,
                _authorizationServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleId = "role-123";

            var request = new AssignPermissionToRoleCommand(
                new List<string>
                {
                    "permission-1",
                    "permission-2"
                },
                roleId);

            _roleManagerMock
                .Setup(x => x.FindByIdAsync(roleId))
                .ReturnsAsync((Role?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "Role not found",
                result.Message);

            Assert.Null(result.Data);

            _roleManagerMock.Verify(
                x => x.FindByIdAsync(roleId),
                Times.Once);

            _authorizationServiceMock.Verify(
                x => x.AssignPermissionToRoleAsync(
                    It.IsAny<List<string>>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldAssignPermissionsAndReturnSuccess_WhenRoleExists()
        {
            // Arrange
            var roleId = "role-123";

            var permissionIds = new List<string>
            {
                "permission-1",
                "permission-2",
                "permission-3"
            };

            var request = new AssignPermissionToRoleCommand(
                permissionIds,
                roleId);

            var role = new Role
            {
                Id = roleId,
                Name = "Technician"
            };

            _roleManagerMock
                .Setup(x => x.FindByIdAsync(roleId))
                .ReturnsAsync(role);

            _authorizationServiceMock
                .Setup(x => x.AssignPermissionToRoleAsync(
                    permissionIds,
                    roleId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "Permissions assigned to role successfully.",
                result.Data);

            _roleManagerMock.Verify(
                x => x.FindByIdAsync(roleId),
                Times.Once);

            _authorizationServiceMock.Verify(
                x => x.AssignPermissionToRoleAsync(
                    permissionIds,
                    roleId),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectRoleIdAndPermissionIds()
        {
            // Arrange
            var roleId = "role-456";

            var permissionIds = new List<string>
            {
                "permission-10",
                "permission-20"
            };

            var request = new AssignPermissionToRoleCommand(
                permissionIds,
                roleId);

            var role = new Role
            {
                Id = roleId,
                Name = "Admin"
            };

            _roleManagerMock
                .Setup(x => x.FindByIdAsync(roleId))
                .ReturnsAsync(role);

            _authorizationServiceMock
                .Setup(x => x.AssignPermissionToRoleAsync(
                    permissionIds,
                    roleId))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _authorizationServiceMock.Verify(
                x => x.AssignPermissionToRoleAsync(
                    permissionIds,
                    roleId),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotCallAuthorizationService_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleId = "invalid-role";

            var request = new AssignPermissionToRoleCommand(
                new List<string> { "permission-1" },
                roleId);

            _roleManagerMock
                .Setup(x => x.FindByIdAsync(roleId))
                .ReturnsAsync((Role?)null);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _authorizationServiceMock.Verify(
                x => x.AssignPermissionToRoleAsync(
                    It.IsAny<List<string>>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldPropagateException_WhenAssignPermissionFails()
        {
            // Arrange
            var roleId = "role-123";

            var permissionIds = new List<string>
            {
                "permission-1"
            };

            var request = new AssignPermissionToRoleCommand(
                permissionIds,
                roleId);

            var role = new Role
            {
                Id = roleId,
                Name = "Admin"
            };

            _roleManagerMock
                .Setup(x => x.FindByIdAsync(roleId))
                .ReturnsAsync(role);

            _authorizationServiceMock
                .Setup(x => x.AssignPermissionToRoleAsync(
                    permissionIds,
                    roleId))
                .ThrowsAsync(
                    new Exception("Failed to assign permissions."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(
                    request,
                    CancellationToken.None));

            Assert.Equal(
                "Failed to assign permissions.",
                exception.Message);

            _authorizationServiceMock.Verify(
                x => x.AssignPermissionToRoleAsync(
                    permissionIds,
                    roleId),
                Times.Once);
        }
    }
}

