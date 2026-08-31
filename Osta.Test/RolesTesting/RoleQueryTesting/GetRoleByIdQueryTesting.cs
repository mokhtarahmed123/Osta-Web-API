using AutoMapper;
using Moq;
using Osta.Core.Feature.Roles.Query.Handler;
using Osta.Core.Feature.Roles.Query.Model;
using Osta.Core.Feature.Roles.Query.Result;
using Osta.Data.Entities.Identity;
using Osta.Identity.Roles;
using System.Net;

namespace Osta.Test.RolesTesting.RoleQueryTesting
{
    public class GetRoleByIdQueryTesting
    {
        private readonly Mock<IRoleService> roleServiceMock;
        private readonly Mock<IMapper> mapperMock;

        private readonly GetRoleByIdQueryHandler handler;

        public GetRoleByIdQueryTesting()
        {
            roleServiceMock = new Mock<IRoleService>();
            mapperMock = new Mock<IMapper>();

            handler = new GetRoleByIdQueryHandler(
                roleServiceMock.Object,
                mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();

            var request = new GetRoleByIdQuery(roleId)
            {
                Id = roleId
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
                x => x.Map<GetRoleByIdResult>(
                    It.IsAny<Role>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessWithMappedRole_WhenRoleExists()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();

            var request = new GetRoleByIdQuery(roleId)
            {
                Id = roleId
            };

            var role = new Role
            {
                Id = roleId
            };

            var mappedRole = new GetRoleByIdResult();

            roleServiceMock
                .Setup(x => x.GetRoleByIdAsync(roleId))
                .ReturnsAsync(role);

            mapperMock
                .Setup(x => x.Map<GetRoleByIdResult>(role))
                .Returns(mappedRole);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Same(mappedRole, result.Data);

            roleServiceMock.Verify(
                x => x.GetRoleByIdAsync(roleId),
                Times.Once);

            mapperMock.Verify(
                x => x.Map<GetRoleByIdResult>(role),
                Times.Once);
        }
    }
}