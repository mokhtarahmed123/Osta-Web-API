using Moq;
using Osta.Core.Feature.Technician.Command.Handler.TechnicianPayoutCommandHandler;
using Osta.Core.Feature.Technician.Command.Model.TechnicianPayout;
using Osta.Domain.Entities.Technician;
using Osta.Domain.Enum;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.TechnicianPayoutTesting.TechnicianPayoutCommandTesting
{
    public class RequestPayoutCommandHandlerTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ITechnicianPayoutService> _technicianPayoutServiceMock;
        private readonly Mock<ITechnicianWalletService> _technicianWalletServiceMock;

        private readonly RequestPayoutCommandHandler _handler;

        public RequestPayoutCommandHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _technicianPayoutServiceMock = new Mock<ITechnicianPayoutService>();
            _technicianWalletServiceMock = new Mock<ITechnicianWalletService>();

            _handler = new RequestPayoutCommandHandler(
                _currentUserServiceMock.Object,
                _technicianPayoutServiceMock.Object,
                _technicianWalletServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns((string)null);

            var command = new RequestPayoutCommand(
                100,
                PayoutMethod.BankTransfer,
                "01000000000"
            );

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("User is not authenticated.", result.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIdIsEmpty()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var command = new RequestPayoutCommand(
                100,
                PayoutMethod.BankTransfer,
                "01000000000"
            );

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("User is not authenticated.", result.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenWalletDoesNotExist()
        {
            // Arrange
            var technicianId = "tech-123";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            _technicianWalletServiceMock
                .Setup(x => x.GetWalletAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((TechnicianWallet)null);

            var command = new RequestPayoutCommand(
                100,
                PayoutMethod.BankTransfer,
                "01000000000"
            );

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(
                "Technician wallet was not found.",
                result.Message);

            _technicianWalletServiceMock.Verify(
                x => x.GetWalletAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _technicianPayoutServiceMock.Verify(
                x => x.GetTechnicianPayoutsAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenWalletBalanceIsInsufficient()
        {
            // Arrange
            var technicianId = "tech-123";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var wallet = new TechnicianWallet
            {
                Amount = 50
            };

            _technicianWalletServiceMock
                .Setup(x => x.GetWalletAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(wallet);

            var command = new RequestPayoutCommand(
                100,
                PayoutMethod.BankTransfer,
                "01000000000"
            );

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(
                "Insufficient wallet balance.",
                result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.GetTechnicianPayoutsAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _technicianPayoutServiceMock.Verify(
                x => x.RequestPayoutAsync(
                    It.IsAny<string>(),
                    It.IsAny<decimal>(),
                    It.IsAny<PayoutMethod>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenPendingPayoutWithSameAmountExists()
        {
            // Arrange
            var technicianId = "tech-123";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var wallet = new TechnicianWallet
            {
                Amount = 500
            };

            _technicianWalletServiceMock
                .Setup(x => x.GetWalletAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(wallet);

            var pendingPayout = new TechnicianPayout
            {
                Amount = 100,
                Status = PayoutStatus.Pending
            };

            _technicianPayoutServiceMock
                .Setup(x => x.GetTechnicianPayoutsAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TechnicianPayout>
                {
                    pendingPayout
                });

            var command = new RequestPayoutCommand(
                100,
                PayoutMethod.BankTransfer,
                "01000000000"
            );

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "You already have a pending payout with the same amount.",
                result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.RequestPayoutAsync(
                    It.IsAny<string>(),
                    It.IsAny<decimal>(),
                    It.IsAny<PayoutMethod>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCreatePayout_WhenNoPendingSameAmountExists()
        {
            // Arrange
            var technicianId = "tech-123";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var wallet = new TechnicianWallet
            {
                Amount = 500
            };

            _technicianWalletServiceMock
                .Setup(x => x.GetWalletAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(wallet);

            _technicianPayoutServiceMock
                .Setup(x => x.GetTechnicianPayoutsAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TechnicianPayout>());

            var payout = new TechnicianPayout
            {
                Id = 15,
                Amount = 100,
                Status = PayoutStatus.Pending
            };

            _technicianPayoutServiceMock
                .Setup(x => x.RequestPayoutAsync(
                    technicianId,
                    100,
                    PayoutMethod.BankTransfer,
                    "01000000000",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payout);

            var command = new RequestPayoutCommand(
                100,
                PayoutMethod.BankTransfer,
                "01000000000"
            );

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            //Assert.Equal(
            //    "Payout request #15 created successfully.",
            //    result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.RequestPayoutAsync(
                    technicianId,
                    100,
                    PayoutMethod.BankTransfer,
                    "01000000000",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCreatePayout_WhenPendingPayoutHasDifferentAmount()
        {
            // Arrange
            var technicianId = "tech-123";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var wallet = new TechnicianWallet
            {
                Amount = 1000
            };

            _technicianWalletServiceMock
                .Setup(x => x.GetWalletAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(wallet);

            var existingPendingPayout = new TechnicianPayout
            {
                Id = 10,
                Amount = 200,
                Status = PayoutStatus.Pending
            };

            _technicianPayoutServiceMock
                .Setup(x => x.GetTechnicianPayoutsAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TechnicianPayout>
                {
                    existingPendingPayout
                });

            var newPayout = new TechnicianPayout
            {
                Id = 20,
                Amount = 100,
                Status = PayoutStatus.Pending
            };

            _technicianPayoutServiceMock
                .Setup(x => x.RequestPayoutAsync(
                    technicianId,
                    100,
                    PayoutMethod.BankTransfer,
                    "01000000000",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newPayout);

            var command = new RequestPayoutCommand(
                100,
                PayoutMethod.BankTransfer,
                "01000000000"
            );

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            //Assert.Equal(
            //    "Payout request #20 created successfully.",
            //    result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.RequestPayoutAsync(
                    technicianId,
                    100,
                    PayoutMethod.BankTransfer,
                    "01000000000",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCreatePayout_WhenExistingPayoutIsNotPending()
        {
            // Arrange
            var technicianId = "tech-123";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var wallet = new TechnicianWallet
            {
                Amount = 500
            };

            _technicianWalletServiceMock
                .Setup(x => x.GetWalletAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(wallet);

            var completedPayout = new TechnicianPayout
            {
                Id = 10,
                Amount = 100,
                Status = PayoutStatus.Completed
            };

            _technicianPayoutServiceMock
                .Setup(x => x.GetTechnicianPayoutsAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TechnicianPayout>
                {
                    completedPayout
                });

            var newPayout = new TechnicianPayout
            {
                Id = 25,
                Amount = 100,
                Status = PayoutStatus.Pending
            };

            _technicianPayoutServiceMock
                .Setup(x => x.RequestPayoutAsync(
                    technicianId,
                    100,
                    PayoutMethod.BankTransfer,
                    "01000000000",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newPayout);

            var command = new RequestPayoutCommand(
                100,
                PayoutMethod.BankTransfer,
                "01000000000"
            );

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            //    Assert.Equal(
            //        "Payout request #25 created successfully.",
            //        result.Message);
        }
    }
}