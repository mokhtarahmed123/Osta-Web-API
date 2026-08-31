
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Core.Feature.Authorization.Command.Handler;
using Osta.Core.Feature.Authorization.Command.Model.Roles;
using Osta.Data.Entities.Identity;
using Osta.Identity.Authorization;

namespace Osta.Test.AuthorizationTesting.AuthorizationCommandTesting
{
    public class AssignRoleToUserCommandHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<Role>> _roleManagerMock;
        private readonly Mock<IAuthorizationService> _authorizationServiceMock;

        private readonly AssignRoleToUserCommandHandler _handler;

        public AssignRoleToUserCommandHandlerTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null!, null!, null!, null!, null!, null!, null!, null!);

            _roleManagerMock = new Mock<RoleManager<Role>>(
                Mock.Of<IRoleStore<Role>>(),
                null!, null!, null!, null!);

            _authorizationServiceMock = new Mock<IAuthorizationService>();

            _handler = new AssignRoleToUserCommandHandler(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _authorizationServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new AssignRoleToUserCommand(
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
            Assert.NotNull(result);

            Assert.Equal(
                "User not found",
                result.Message);

            Assert.Null(result.Data);

            _userManagerMock.Verify(
                x => x.FindByIdAsync(request.UserId),
                Times.Once);

            _roleManagerMock.Verify(
                x => x.FindByIdAsync(It.IsAny<string>()),
                Times.Never);

            _authorizationServiceMock.Verify(
                x => x.AssignRoleToUserAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var request = new AssignRoleToUserCommand(
                "user-123",
                "role-123");

            var user = new User
            {
                Id = request.UserId,
                UserName = "test@example.com",
                Email = "test@example.com"
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync(request.UserId))
                .ReturnsAsync(user);

            _roleManagerMock
                .Setup(x => x.FindByIdAsync(request.RoleId))
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

            _userManagerMock.Verify(
                x => x.FindByIdAsync(request.UserId),
                Times.Once);

            _roleManagerMock.Verify(
                x => x.FindByIdAsync(request.RoleId),
                Times.Once);

            _authorizationServiceMock.Verify(
                x => x.AssignRoleToUserAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenAssignRoleFails()
        {
            // Arrange
            var request = new AssignRoleToUserCommand(
                "user-123",
                "role-123");

            var user = new User
            {
                Id = request.UserId,
                UserName = "test@example.com",
                Email = "test@example.com"
            };

            var role = new Role
            {
                Id = request.RoleId,
                Name = "Technician"
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync(request.UserId))
                .ReturnsAsync(user);

            _roleManagerMock
                .Setup(x => x.FindByIdAsync(request.RoleId))
                .ReturnsAsync(role);

            _authorizationServiceMock
                .Setup(x => x.AssignRoleToUserAsync(
                    role.Id,
                    user.Id))
                .ReturnsAsync(IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Role assignment failed"
                    }));

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "Failed to assign role",
                result.Message);

            Assert.Null(result.Data);

            _authorizationServiceMock.Verify(
                x => x.AssignRoleToUserAsync(
                    role.Id,
                    user.Id),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenRoleIsAssignedSuccessfully()
        {
            // Arrange
            var request = new AssignRoleToUserCommand(
                "user-123",
                "role-123");

            var user = new User
            {
                Id = request.UserId,
                UserName = "test@example.com",
                Email = "test@example.com"
            };

            var role = new Role
            {
                Id = request.RoleId,
                Name = "Technician"
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync(request.UserId))
                .ReturnsAsync(user);

            _roleManagerMock
                .Setup(x => x.FindByIdAsync(request.RoleId))
                .ReturnsAsync(role);

            _authorizationServiceMock
                .Setup(x => x.AssignRoleToUserAsync(
                    role.Id,
                    user.Id))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "Role assigned successfully",
                result.Data);

            _userManagerMock.Verify(
                x => x.FindByIdAsync(request.UserId),
                Times.Once);

            _roleManagerMock.Verify(
                x => x.FindByIdAsync(request.RoleId),
                Times.Once);

            _authorizationServiceMock.Verify(
                x => x.AssignRoleToUserAsync(
                    role.Id,
                    user.Id),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectIds_WhenAssigningRole()
        {
            // Arrange
            var userId = "user-999";
            var roleId = "role-999";

            var request = new AssignRoleToUserCommand(
                userId,
                roleId);

            var user = new User
            {
                Id = userId,
                UserName = "user@test.com",
                Email = "user@test.com"
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
                .Setup(x => x.AssignRoleToUserAsync(
                    roleId,
                    userId))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _authorizationServiceMock.Verify(
                x => x.AssignRoleToUserAsync(
                    roleId,
                    userId),
                Times.Once);
        }
    }
}

