using Moq;
using Osta.Core.Feature.Technician.Command.Handler.TechnicianPayoutCommandHandler;
using Osta.Core.Feature.Technician.Command.Model.TechnicianPayout;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.TechnicianPayoutTesting.TechnicianPayoutCommandTesting
{
    public class CancelPayoutCommandHandlerTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ITechnicianPayoutService> _technicianPayoutServiceMock;

        private readonly CancelPayoutCommandHandler _handler;

        public CancelPayoutCommandHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _technicianPayoutServiceMock = new Mock<ITechnicianPayoutService>();

            _handler = new CancelPayoutCommandHandler(
                _currentUserServiceMock.Object,
                _technicianPayoutServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns((string)null);

            var request = new CancelPayoutCommand(1);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(
                "User is not authenticated.",
                result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.CancelPayoutAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIdIsEmpty()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var request = new CancelPayoutCommand(1);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(
                "User is not authenticated.",
                result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.CancelPayoutAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIdIsWhitespace()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("   ");

            var request = new CancelPayoutCommand(10);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(
                "User is not authenticated.",
                result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.CancelPayoutAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCancelPayoutAndReturnSuccess_WhenUserIsAuthenticated()
        {
            // Arrange
            var technicianId = "tech-123";
            var payoutId = 10;

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            _technicianPayoutServiceMock
              .Setup(x => x.CancelPayoutAsync(
                  payoutId,
                  technicianId,
                  It.IsAny<CancellationToken>()))
              .ReturnsAsync(true);

            var request = new CancelPayoutCommand(payoutId);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            //Assert.Equal(
            //    "Payout cancelled successfully.",
            //    result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.CancelPayoutAsync(
                    payoutId,
                    technicianId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectPayoutIdAndTechnicianId()
        {
            // Arrange
            var technicianId = "technician-456";
            var payoutId = 25;

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            _technicianPayoutServiceMock
                .Setup(x => x.CancelPayoutAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var request = new CancelPayoutCommand(payoutId);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _technicianPayoutServiceMock.Verify(
                x => x.CancelPayoutAsync(
                    payoutId,
                    technicianId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}