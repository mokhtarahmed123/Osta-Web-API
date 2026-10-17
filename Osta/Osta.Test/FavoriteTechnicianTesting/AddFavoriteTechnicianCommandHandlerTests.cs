using Moq;
using Osta.Core.Feature.FavoriteTechnician.Command.Handler;
using Osta.Core.Feature.FavoriteTechnician.Command.Model;
using Osta.Service.Abstract.CustomerAbstract;
using Osta.SharedKernel.Identity;
using System.Net;

namespace Osta.Test.FavoriteTechnicianTesting
{
    public class AddFavoriteTechnicianCommandHandlerTests
    {
        private readonly Mock<IFavoriteTechnicianService> _favoriteTechnicianServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly AddFavoriteTechnicianCommandHandler _handler;

        public AddFavoriteTechnicianCommandHandlerTests()
        {
            _favoriteTechnicianServiceMock = new Mock<IFavoriteTechnicianService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new AddFavoriteTechnicianCommandHandler(
                _favoriteTechnicianServiceMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            // Act
            Func<Task> act = async () => await _handler.Handle(null, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_EmptyOrWhitespaceTechnicianId_ReturnsBadRequest(string technicianId)
        {
            // Arrange
            var request = new AddFavoriteTechnicianCommand(technicianId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Technician Id is required.", result.Message);
            _favoriteTechnicianServiceMock.Verify(
                s => s.Add(It.IsAny<Data.Entities.FavoriteTechnician>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_UserNotAuthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new AddFavoriteTechnicianCommand("tech-1");
            _currentUserServiceMock.Setup(s => s.UserId).Returns((string)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
            _favoriteTechnicianServiceMock.Verify(
                s => s.Add(It.IsAny<Data.Entities.FavoriteTechnician>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidRequest_AddsFavoriteAndReturnsSuccess()
        {
            // Arrange
            var request = new AddFavoriteTechnicianCommand("tech-1");
            const string customerId = "customer-1";
            _currentUserServiceMock.Setup(s => s.UserId).Returns(customerId);

            //Data.Entities.FavoriteTechnician capturedEntity = null;
            _favoriteTechnicianServiceMock
                .Setup(s => s.Add(It.IsAny<Data.Entities.FavoriteTechnician>(), CancellationToken.None))
                //.Callback<Osta.Data.Entities.FavoriteTechnician>(f => capturedEntity = f)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(result.StatusCode, HttpStatusCode.OK);

            _favoriteTechnicianServiceMock.Verify(
                s => s.Add(
                    It.Is<Data.Entities.FavoriteTechnician>(
                        f => f.CustomerId == customerId &&
                             f.TechnicianId == "tech-1"),
                    CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ServiceThrowsInvalidOperationException_ReturnsBadRequest()
        {
            // Arrange
            var request = new AddFavoriteTechnicianCommand("tech-1");
            _currentUserServiceMock.Setup(s => s.UserId).Returns("customer-1");

            _favoriteTechnicianServiceMock
                .Setup(s => s.Add(It.IsAny<Data.Entities.FavoriteTechnician>(), CancellationToken.None))
                .ThrowsAsync(new InvalidOperationException("Technician already in favorites."));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Technician already in favorites.", result.Message);
        }
    }
}