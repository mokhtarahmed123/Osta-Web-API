using AutoMapper;
using Moq;
using Osta.Core.Feature.Roles.Command.Handler;
using Osta.Core.Feature.Roles.Command.Model;
using Osta.Data.Entities.Identity;
using Osta.Identity.Roles;
using System.Net;

namespace Osta.Test.RolesTesting.RoleCommandTesting
{
    public class AddRoleCommandTesting
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IRoleService> roleService;
        private readonly AddRoleCommandHandler handler;
        public AddRoleCommandTesting()
        {
            mapperMock = new Mock<IMapper>();
            roleService = new Mock<IRoleService>();
            handler = new AddRoleCommandHandler(
                mapperMock.Object, roleService.Object
                );

        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenRoleIsCreatedSuccessfully()
        {
            var request = new AddRoleCommand();

            var role = new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = "role"

            };
            mapperMock
                .Setup(x => x.Map<Role>(request))
                .Returns(role);


            roleService
                .Setup(x => x.CreateRoleAsync(role))
                .ReturnsAsync(true);

            var result = await handler.Handle(
           request,
          CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Role added successfully.", result.Data);

            mapperMock.Verify(
                x => x.Map<Role>(request),
                Times.Once);

            roleService.Verify(
                x => x.CreateRoleAsync(role),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenRoleCreationFails()
        {
            // Arrange
            var request = new AddRoleCommand();

            var role = new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = "role"
            };

            mapperMock
                .Setup(x => x.Map<Role>(request))
                .Returns(role);

            roleService
                .Setup(x => x.CreateRoleAsync(role))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            mapperMock.Verify(
                x => x.Map<Role>(request),
                Times.Once);

            roleService.Verify(
                x => x.CreateRoleAsync(role),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnServerError_WhenExceptionOccurs()
        {
            // Arrange
            var request = new AddRoleCommand();

            mapperMock
                .Setup(x => x.Map<Role>(request))
                .Throws(new Exception("Mapping failed"));

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);

            mapperMock.Verify(
                x => x.Map<Role>(request),
                Times.Once);

            roleService.Verify(
                x => x.CreateRoleAsync(It.IsAny<Role>()),
                Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldReturnServerError_WhenRoleServiceThrowsException()
        {
            // Arrange
            var request = new AddRoleCommand();

            var role = new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = "role"
            };

            mapperMock
                .Setup(x => x.Map<Role>(request))
                .Returns(role);

            roleService
                .Setup(x => x.CreateRoleAsync(role))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);

            mapperMock.Verify(
                x => x.Map<Role>(request),
                Times.Once);

            roleService.Verify(
                x => x.CreateRoleAsync(role),
                Times.Once);
        }
    }
}
