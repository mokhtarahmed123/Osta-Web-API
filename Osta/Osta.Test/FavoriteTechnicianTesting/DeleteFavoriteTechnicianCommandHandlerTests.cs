using Moq;
using Osta.Core.Feature.FavoriteTechnician.Command.Handler;
using Osta.Core.Feature.FavoriteTechnician.Command.Model;
using Osta.Service.Abstract.CustomerAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.FavoriteTechnicianTesting
{
    public class DeleteFavoriteTechnicianCommandHandlerTests
    {
        private readonly Mock<IFavoriteTechnicianService> _favoriteTechnicianServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly DeleteFavoriteTechnicianCommandHandler _handler;

        public DeleteFavoriteTechnicianCommandHandlerTests()
        {
            _favoriteTechnicianServiceMock = new Mock<IFavoriteTechnicianService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new DeleteFavoriteTechnicianCommandHandler(
                _favoriteTechnicianServiceMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            Func<Task> act = async () => await _handler.Handle(null, CancellationToken.None);
            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_EmptyOrWhitespaceTechnicianId_ReturnsBadRequest(string technicianId)
        {
            // Arrange
            var request = new DeleteFavoriteTechnicianCommand(technicianId) { TechnicianId = technicianId };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Technician Id is required.", result.Message);
            _favoriteTechnicianServiceMock.Verify(
                s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_UserNotAuthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new DeleteFavoriteTechnicianCommand("tech-1") { TechnicianId = "tech-1" };
            _currentUserServiceMock.Setup(s => s.UserId).Returns((string)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
            _favoriteTechnicianServiceMock.Verify(
                s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidRequest_DeletesFavoriteAndReturnsSuccess()
        {
            // Arrange
            var request = new DeleteFavoriteTechnicianCommand("tech-1") { TechnicianId = "tech-1" };
            const string customerId = "customer-1";
            _currentUserServiceMock.Setup(s => s.UserId).Returns(customerId);
            _favoriteTechnicianServiceMock
                .Setup(s => s.Delete(customerId, "tech-1", CancellationToken.None))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Success", result.Message);
            _favoriteTechnicianServiceMock.Verify(
                s => s.Delete(customerId, "tech-1", CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_ServiceThrowsKeyNotFoundException_ReturnsNotFound()
        {
            // Arrange
            var request = new DeleteFavoriteTechnicianCommand("tech-1") { TechnicianId = "tech-1" };
            _currentUserServiceMock.Setup(s => s.UserId).Returns("customer-1");
            _favoriteTechnicianServiceMock
                .Setup(s => s.Delete("customer-1", "tech-1", CancellationToken.None))
                .ThrowsAsync(new KeyNotFoundException("Favorite technician not found."));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Favorite technician not found.", result.Message);
        }
    }
}