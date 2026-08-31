using Moq;
using Osta.Core.Feature.Technician.Command.Handler.TechnicianPayoutCommandHandler;
using Osta.Core.Feature.Technician.Command.Model.TechnicianPayout;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.TechnicianPayoutTesting.TechnicianPayoutCommandTesting
{
    public class RejectPayoutCommandHandlerTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ITechnicianPayoutService> _technicianPayoutServiceMock;

        private readonly RejectPayoutCommandHandler _handler;

        public RejectPayoutCommandHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _technicianPayoutServiceMock = new Mock<ITechnicianPayoutService>();

            _handler = new RejectPayoutCommandHandler(
                _currentUserServiceMock.Object,
                _technicianPayoutServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns((string)null);

            var request = new RejectPayoutCommand(
                1,
                "Invalid payout details");

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
                x => x.RejectPayoutAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenPayoutIdIsInvalid()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-123");

            var request = new RejectPayoutCommand(
                0,
                "Invalid payout details");

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(
                "Invalid payout id.",
                result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.RejectPayoutAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenRejectionReasonIsEmpty()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-123");

            var request = new RejectPayoutCommand(
                10,
                string.Empty);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(
                "Rejection reason is required.",
                result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.RejectPayoutAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenRejectionReasonIsWhitespace()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-123");

            var request = new RejectPayoutCommand(
                10,
                "   ");

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(
                "Rejection reason is required.",
                result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.RejectPayoutAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenPayoutDoesNotExist()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-123");

            var payoutId = 10;
            var reason = "Bank account information is invalid";

            _technicianPayoutServiceMock
                .Setup(x => x.RejectPayoutAsync(
                    payoutId,
                    reason,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var request = new RejectPayoutCommand(
                payoutId,
                reason);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(
                "Payout not found.",
                result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.RejectPayoutAsync(
                    payoutId,
                    reason,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenPayoutIsRejected()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-123");

            var payoutId = 10;
            var reason = "Bank account information is invalid";

            _technicianPayoutServiceMock
                .Setup(x => x.RejectPayoutAsync(
                    payoutId,
                    reason,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var request = new RejectPayoutCommand(
                payoutId,
                reason);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            //Assert.Equal(
            //    "Payout rejected successfully.",
            //    result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.RejectPayoutAsync(
                    payoutId,
                    reason,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectArgumentsToService()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("technician-123");

            var payoutId = 25;
            var rejectionReason = "Invalid receiving details";

            _technicianPayoutServiceMock
                .Setup(x => x.RejectPayoutAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var request = new RejectPayoutCommand(
                payoutId,
                rejectionReason);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _technicianPayoutServiceMock.Verify(
                x => x.RejectPayoutAsync(
                    payoutId,
                    rejectionReason,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}