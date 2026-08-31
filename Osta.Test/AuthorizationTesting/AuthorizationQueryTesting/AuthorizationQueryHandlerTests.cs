using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Core.Feature.Authorization.Query.Handler;
using Osta.Core.Feature.Authorization.Query.Model;
using Osta.Core.Feature.Authorization.Query.Model.PermissionModel;
using Osta.Data.Entities.Identity;
using Osta.Identity.Authorization;

namespace Osta.Test.AuthorizationTesting.AuthorizationQueryTesting
{
    public class AuthorizationQueryHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<Role>> _roleManagerMock;
        private readonly Mock<IAuthorizationService> _authorizationServiceMock;

        private readonly AuthorizationQueryHandler _handler;

        public AuthorizationQueryHandlerTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null!, null!, null!, null!, null!, null!, null!, null!);

            _roleManagerMock = new Mock<RoleManager<Role>>(
                Mock.Of<IRoleStore<Role>>(),
                null!, null!, null!, null!);

            _authorizationServiceMock = new Mock<IAuthorizationService>();

            _handler = new AuthorizationQueryHandler(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _authorizationServiceMock.Object);
        }

        // =========================================================
        // UserIsInRoleQuery
        // =========================================================

        [Fact]
        public async Task Handle_UserIsInRole_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new UserIsInRoleQuery(
                "user-123",
                "role-123");

            _userManagerMock
                .Setup(x => x.FindByIdAsync(request.UserId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal("User not found", result.Message);
            Assert.False(result.Data);

            _roleManagerMock.Verify(
                x => x.FindByIdAsync(It.IsAny<string>()),
                Times.Never);

            _authorizationServiceMock.Verify(
                x => x.IsInRoleAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_UserIsInRole_ShouldReturnNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var userId = "user-123";
            var roleId = "role-123";

            var request = new UserIsInRoleQuery(
                userId,
                roleId);

            var user = new User
            {
                Id = userId,
                UserName = "test@test.com"
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _roleManagerMock
                .Setup(x => x.FindByIdAsync(roleId))
                .ReturnsAsync((Role?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal("Role not found", result.Message);
            Assert.False(result.Data);

            _authorizationServiceMock.Verify(
                x => x.IsInRoleAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_UserIsInRole_ShouldReturnTrue_WhenUserIsInRole()
        {
            // Arrange
            var userId = "user-123";
            var roleId = "role-123";

            var request = new UserIsInRoleQuery(
                userId,
                roleId);

            var user = new User
            {
                Id = userId,
                UserName = "test@test.com"
            };

            var role = new Role
            {
                Id = roleId,
                Name = "Admin"
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _roleManagerMock
                .Setup(x => x.FindByIdAsync(roleId))
                .ReturnsAsync(role);

            _authorizationServiceMock
                .Setup(x => x.IsInRoleAsync(
                    userId,
                    role.Name))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.True(result.Data);
            //Assert.Equal(
            //    "User is in role Admin",
            //    result.Message);

            _authorizationServiceMock.Verify(
                x => x.IsInRoleAsync(
                    userId,
                    role.Name),
                Times.Once);
        }

        [Fact]
        public async Task Handle_UserIsInRole_ShouldReturnFalse_WhenUserIsNotInRole()
        {
            // Arrange
            var userId = "user-123";
            var roleId = "role-123";

            var request = new UserIsInRoleQuery(
                userId,
                roleId);

            var user = new User
            {
                Id = userId,
                UserName = "test@test.com"
            };

            var role = new Role
            {
                Id = roleId,
                Name = "Admin"
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _roleManagerMock
                .Setup(x => x.FindByIdAsync(roleId))
                .ReturnsAsync(role);

            _authorizationServiceMock
                .Setup(x => x.IsInRoleAsync(
                    userId,
                    role.Name))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Data);
            //Assert.Equal(
            //    "User is not in role `Admin`",
            //    result.Message);
        }


        // =========================================================
        // GetUserRolesQuery
        // =========================================================

        [Fact]
        public async Task Handle_GetUserRoles_ShouldReturnNotFound_WhenRolesAreNull()
        {
            // Arrange
            var userId = "user-123";

            var request = new GetUserRolesQuery(userId);

            _authorizationServiceMock
                .Setup(x => x.GetUserRolesAsync(userId))
                .ReturnsAsync((IList<string>?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "User not found.",
                result.Message);

            Assert.Null(result.Data);

            _authorizationServiceMock.Verify(
                x => x.GetUserRolesAsync(userId),
                Times.Once);
        }

        [Fact]
        public async Task Handle_GetUserRoles_ShouldReturnRoles_WhenRolesExist()
        {
            // Arrange
            var userId = "user-123";

            var roles = new List<string>
            {
                "Admin",
                "Technician"
            };

            var request = new GetUserRolesQuery(userId);

            _authorizationServiceMock
                .Setup(x => x.GetUserRolesAsync(userId))
                .ReturnsAsync(roles);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            Assert.Contains("Admin", result.Data);
            Assert.Contains("Technician", result.Data);

            _authorizationServiceMock.Verify(
                x => x.GetUserRolesAsync(userId),
                Times.Once);
        }


        // =========================================================
        // RoleHasPermissionQuery
        // =========================================================

        [Fact]
        public async Task Handle_RoleHasPermission_ShouldReturnTrue_WhenPermissionExists()
        {
            // Arrange
            var request = new RoleHasPermissionQuery(
                "role-123",
                "permission-123");

            _authorizationServiceMock
                .Setup(x => x.RoleHasPermissionAsync(
                    request.RoleId,
                    request.PermissionId))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.True(result.Data);

            _authorizationServiceMock.Verify(
                x => x.RoleHasPermissionAsync(
                    request.RoleId,
                    request.PermissionId),
                Times.Once);
        }

        [Fact]
        public async Task Handle_RoleHasPermission_ShouldReturnFalse_WhenPermissionDoesNotExist()
        {
            // Arrange
            var request = new RoleHasPermissionQuery(
                "role-123",
                "permission-123");

            _authorizationServiceMock
                .Setup(x => x.RoleHasPermissionAsync(
                    request.RoleId,
                    request.PermissionId))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Data);

            _authorizationServiceMock.Verify(
                x => x.RoleHasPermissionAsync(
                    request.RoleId,
                    request.PermissionId),
                Times.Once);
        }


        // =========================================================
        // GetRolePermissionsQuery
        // =========================================================

        [Fact]
        public async Task Handle_GetRolePermissions_ShouldReturnNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleId = "role-123";

            var request = new GetRolePermissionsQuery(roleId);

            _roleManagerMock
                .Setup(x => x.FindByIdAsync(roleId))
                .ReturnsAsync((Role?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                $" Role With Id {roleId} Not Found ",
                result.Message);

            Assert.Null(result.Data);

            _authorizationServiceMock.Verify(
                x => x.GetRolePermissionsAsync(
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_GetRolePermissions_ShouldReturnPermissions_WhenRoleExists()
        {
            // Arrange
            var roleId = "role-123";

            var request = new GetRolePermissionsQuery(roleId);

            var role = new Role
            {
                Id = roleId,
                Name = "Admin"
            };

            var permissions = new List<string>
            {
                "CreateUser",
                "DeleteUser",
                "UpdateUser"
            };

            _roleManagerMock
                .Setup(x => x.FindByIdAsync(roleId))
                .ReturnsAsync(role);

            _authorizationServiceMock
                .Setup(x => x.GetRolePermissionsAsync(roleId))
                .ReturnsAsync(permissions);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(3, result.Data.Count);
            Assert.Contains("CreateUser", result.Data);
            Assert.Contains("DeleteUser", result.Data);
            Assert.Contains("UpdateUser", result.Data);

            _authorizationServiceMock.Verify(
                x => x.GetRolePermissionsAsync(roleId),
                Times.Once);
        }


        // =========================================================
        // GetPermissionRolesQuery
        // =========================================================

        [Fact]
        public async Task Handle_GetPermissionRoles_ShouldReturnNotFound_WhenPermissionDoesNotExist()
        {
            // Arrange
            var permissionId = "permission-123";

            var request = new GetPermissionRolesQuery(
                permissionId);

            _authorizationServiceMock
                .Setup(x => x.PermissionExistAsync(permissionId))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                " Permission Not Found ",
                result.Message);

            Assert.Null(result.Data);

            _authorizationServiceMock.Verify(
                x => x.PermissionExistAsync(permissionId),
                Times.Once);

            _authorizationServiceMock.Verify(
                x => x.GetPermissionRolesAsync(
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_GetPermissionRoles_ShouldReturnRoles_WhenPermissionExists()
        {
            // Arrange
            var permissionId = "permission-123";

            var request = new GetPermissionRolesQuery(
                permissionId);

            var roles = new List<string>
            {
                "Admin",
                "Technician"
            };

            _authorizationServiceMock
                .Setup(x => x.PermissionExistAsync(permissionId))
                .ReturnsAsync(true);

            _authorizationServiceMock
                .Setup(x => x.GetPermissionRolesAsync(permissionId))
                .ReturnsAsync(roles);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            Assert.Contains("Admin", result.Data);
            Assert.Contains("Technician", result.Data);

            _authorizationServiceMock.Verify(
                x => x.PermissionExistAsync(permissionId),
                Times.Once);

            _authorizationServiceMock.Verify(
                x => x.GetPermissionRolesAsync(permissionId),
                Times.Once);
        }
    }
}

