using AutoMapper;
using Moq;
using Osta.Core.Feature.FavoriteTechnician.Query.Handler;
using Osta.Core.Feature.FavoriteTechnician.Query.Model;
using Osta.Core.Feature.FavoriteTechnician.Query.Result;
using Osta.Service.Abstract.CustomerAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.FavoriteTechnicianTesting
{
    public class FavoriteTechnicianQueryHandlerTests
    {
        private readonly Mock<IFavoriteTechnicianService> _favoriteTechnicianServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly FavoriteTechnicianQueryHandler _handler;

        public FavoriteTechnicianQueryHandlerTests()
        {
            _favoriteTechnicianServiceMock = new Mock<IFavoriteTechnicianService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _mapperMock = new Mock<IMapper>();
            _handler = new FavoriteTechnicianQueryHandler(
                _favoriteTechnicianServiceMock.Object,
                _currentUserServiceMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_UserNotAuthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _currentUserServiceMock.Setup(s => s.UserId).Returns((string)null);
            var request = new GetMyFavoriteQuery();

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
            _favoriteTechnicianServiceMock.Verify(
                s => s.GetMyFavorites(It.IsAny<string>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidUser_ReturnsMappedFavoritesSuccessfully()
        {
            // Arrange
            const string customerId = "customer-1";
            _currentUserServiceMock.Setup(s => s.UserId).Returns(customerId);

            var favoritesFromService = new List<Data.Entities.FavoriteTechnician>
            {
                new Data.Entities.FavoriteTechnician { CustomerId = customerId, TechnicianId = "tech-1" },
                new Data.Entities.FavoriteTechnician { CustomerId = customerId, TechnicianId = "tech-2" }
            };
            _favoriteTechnicianServiceMock
                .Setup(s => s.GetMyFavorites(customerId, CancellationToken.None))
                .ReturnsAsync(favoritesFromService);

            var mappedResult = new List<GetMyFavoriteResult>
            {
                new GetMyFavoriteResult { TechnicianId = "tech-1" },
                new GetMyFavoriteResult { TechnicianId = "tech-2" }
            };
            _mapperMock
                .Setup(m => m.Map<List<GetMyFavoriteResult>>(favoritesFromService))
                .Returns(mappedResult);

            var request = new GetMyFavoriteQuery();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal("tech-1", result.Data[0].TechnicianId);
            _favoriteTechnicianServiceMock.Verify(s => s.GetMyFavorites(customerId, CancellationToken.None), Times.Once);
            _mapperMock.Verify(m => m.Map<List<GetMyFavoriteResult>>(favoritesFromService), Times.Once);
        }

        [Fact]
        public async Task Handle_NoFavorites_ReturnsSuccessWithEmptyList()
        {
            // Arrange
            const string customerId = "customer-1";
            _currentUserServiceMock.Setup(s => s.UserId).Returns(customerId);

            var favoritesFromService = new List<Data.Entities.FavoriteTechnician>();
            _favoriteTechnicianServiceMock
                .Setup(s => s.GetMyFavorites(customerId, CancellationToken.None))
                .ReturnsAsync(favoritesFromService);

            var mappedResult = new List<GetMyFavoriteResult>();
            _mapperMock
                .Setup(m => m.Map<List<GetMyFavoriteResult>>(favoritesFromService))
                .Returns(mappedResult);

            var request = new GetMyFavoriteQuery();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Empty(result.Data);
        }
    }
}