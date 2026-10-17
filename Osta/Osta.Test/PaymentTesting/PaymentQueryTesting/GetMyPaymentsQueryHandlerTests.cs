using Moq;
using Osta.Core.Feature.Payment.Query;
using Osta.SharedKernel.Identity;

namespace Osta.Test.PaymentTesting.PaymentQueryTesting
{
    public class GetMyPaymentsQueryHandlerTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IPaymentQueryService> _paymentQueryServiceMock;

        private readonly GetMyPaymentsQueryHandler _handler;

        public GetMyPaymentsQueryHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _paymentQueryServiceMock = new Mock<IPaymentQueryService>();

            _handler = new GetMyPaymentsQueryHandler(
                _currentUserServiceMock.Object,
                _paymentQueryServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns((string?)null);

            var query = new GetMyPaymentsQuery();

            // Act
            var result = await _handler.Handle(
                query,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);

            Assert.Equal(
                "User is not authenticated.",
                result.Message);

            _paymentQueryServiceMock.Verify(
                x => x.GetMyPaymentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessWithEmptyList_WhenUserHasNoPayments()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _paymentQueryServiceMock
                .Setup(x => x.GetMyPaymentsAsync(
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GetMyPaymentsResult>());

            // Act
            var result = await _handler.Handle(
                new GetMyPaymentsQuery(),
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);

            _paymentQueryServiceMock.Verify(
                x => x.GetMyPaymentsAsync(
                    userId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnUserPayments()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var payments = new List<GetMyPaymentsResult>
            {
                new GetMyPaymentsResult
                {
                    Id = 1,
                    BookingId = 10,
                    Amount = 500,
                    Status = "Completed",
                    Method = "Card",
                    TransactionId = "pi_test_123",
                    CreatedAt = new DateTime(2026, 8, 30),
                    TechnicianName = "Ahmed"
                },
                new GetMyPaymentsResult
                {
                    Id = 2,
                    BookingId = 20,
                    Amount = 1000,
                    Status = "Pending",
                    Method = "Card",
                    TransactionId = "pi_test_456",
                    CreatedAt = new DateTime(2026, 8, 29),
                    TechnicianName = "Mohamed"
                }
            };

            _paymentQueryServiceMock
                .Setup(x => x.GetMyPaymentsAsync(
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payments);

            // Act
            var result = await _handler.Handle(
                new GetMyPaymentsQuery(),
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            Assert.Equal(2, result.Data.Count);

            Assert.Equal(1, result.Data[0].Id);
            Assert.Equal(10, result.Data[0].BookingId);
            Assert.Equal(500, result.Data[0].Amount);
            Assert.Equal("Completed", result.Data[0].Status);
            Assert.Equal("Card", result.Data[0].Method);
            Assert.Equal("pi_test_123", result.Data[0].TransactionId);
            Assert.Equal("Ahmed", result.Data[0].TechnicianName);

            _paymentQueryServiceMock.Verify(
                x => x.GetMyPaymentsAsync(
                    userId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCurrentUserIdToService()
        {
            // Arrange
            var userId = "customer123";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _paymentQueryServiceMock
                .Setup(x => x.GetMyPaymentsAsync(
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GetMyPaymentsResult>());

            // Act
            await _handler.Handle(
                new GetMyPaymentsQuery(),
                CancellationToken.None);

            // Assert
            _paymentQueryServiceMock.Verify(
                x => x.GetMyPaymentsAsync(
                    userId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaymentsWithCorrectData()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var payments = new List<GetMyPaymentsResult>
            {
                new GetMyPaymentsResult
                {
                    Id = 5,
                    BookingId = 10,
                    Amount = 750,
                    Status = "Completed",
                    Method = "Card",
                    TransactionId = "pi_test_123",
                    CreatedAt = new DateTime(2026, 8, 30),
                    TechnicianName = "Test Technician"
                }
            };

            _paymentQueryServiceMock
                .Setup(x => x.GetMyPaymentsAsync(
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payments);

            // Act
            var result = await _handler.Handle(
                new GetMyPaymentsQuery(),
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);

            var payment = Assert.Single(result.Data);

            Assert.Equal(5, payment.Id);
            Assert.Equal(10, payment.BookingId);
            Assert.Equal(750, payment.Amount);
            Assert.Equal("Completed", payment.Status);
            Assert.Equal("Card", payment.Method);
            Assert.Equal("pi_test_123", payment.TransactionId);
            Assert.Equal("Test Technician", payment.TechnicianName);
        }

        [Fact]
        public async Task Handle_ShouldUseCancellationToken()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var cancellationToken =
                new CancellationTokenSource().Token;

            _paymentQueryServiceMock
                .Setup(x => x.GetMyPaymentsAsync(
                    userId,
                    cancellationToken))
                .ReturnsAsync(new List<GetMyPaymentsResult>());

            // Act
            await _handler.Handle(
                new GetMyPaymentsQuery(),
                cancellationToken);

            // Assert
            _paymentQueryServiceMock.Verify(
                x => x.GetMyPaymentsAsync(
                    userId,
                    cancellationToken),
                Times.Once);
        }
    }
}