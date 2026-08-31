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
    public class GetAllRolesQueryTesting
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IRoleService> roleServiceMock;
        private readonly GetAllRolesQueryHandler handler;
        public GetAllRolesQueryTesting()
        {
            mapperMock = new Mock<IMapper>();
            roleServiceMock = new Mock<IRoleService>();
            handler = new GetAllRolesQueryHandler(
                 roleServiceMock.Object, mapperMock.Object
                );

        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessWithEmptyList_WhenNoRolesExist()
        {
            // Arrange
            var request = new GetAllRolesQuery();

            var roles = new List<Role>();
            var mappedRoles = new List<GetAllRolesResult>();

            roleServiceMock
                .Setup(x => x.GetAllRolesAsync())
                .ReturnsAsync(roles);

            mapperMock
                .Setup(x => x.Map<List<GetAllRolesResult>>(roles))
                .Returns(mappedRoles);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Empty(result.Data);

            roleServiceMock.Verify(
                x => x.GetAllRolesAsync(),
                Times.Once);

            mapperMock.Verify(
                x => x.Map<List<GetAllRolesResult>>(roles),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldThrowException_WhenRoleServiceFails()
        {
            // Arrange
            var request = new GetAllRolesQuery();

            var exception = new Exception("Database error");

            roleServiceMock
                .Setup(x => x.GetAllRolesAsync())
                .ThrowsAsync(exception);

            // Act & Assert
            var result = await Assert.ThrowsAsync<Exception>(
                () => handler.Handle(
                    request,
                    CancellationToken.None));

            Assert.Equal("Database error", result.Message);

            mapperMock.Verify(
                x => x.Map<List<GetAllRolesResult>>(It.IsAny<List<Role>>()),
                Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldReturnSuccessWithMappedRoles_WhenRolesExist()
        {
            // Arrange
            var request = new GetAllRolesQuery();

            var roles = new List<Role>
            {
                new Role
                {
                    Id = Guid.NewGuid().ToString()
                },
                new Role
                {
                    Id = Guid.NewGuid().ToString(),
                }
            };

            var mappedRoles = new List<GetAllRolesResult>
            {
                new GetAllRolesResult(),
                new GetAllRolesResult()
            };

            roleServiceMock
                .Setup(x => x.GetAllRolesAsync())
                .ReturnsAsync(roles);

            mapperMock
                .Setup(x => x.Map<List<GetAllRolesResult>>(roles))
                .Returns(mappedRoles);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Same(mappedRoles, result.Data);

            roleServiceMock.Verify(
                x => x.GetAllRolesAsync(),
                Times.Once);

            mapperMock.Verify(
                x => x.Map<List<GetAllRolesResult>>(roles),
                Times.Once);
        }
    }
}