
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Core.Feature.Payment.Command;
using Osta.Core.HandlerMiddleware;
using Osta.Data.Entities.Identity;
using Osta.Data.Enum;
using Osta.Domain.Entities.Customer;
using Osta.Domain.Enum;
using Osta.Infrastructure.Abstract.PaymentAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Payment.CouponService;
using Osta.Payment.Model;
using Osta.Payment.Services;
using Osta.SharedKernel.Identity;

namespace Osta.Test.CoreTetsing.PaymentTesting.Command
{
    public class CreatePaymentIntentCommandHandlerTests
    {
        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ICouponService> _couponServiceMock;

        private readonly CreatePaymentIntentCommandHandler _handler;

        public CreatePaymentIntentCommandHandlerTests()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _couponServiceMock = new Mock<ICouponService>();

            var userStoreMock = new Mock<IUserStore<User>>();

            _userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);

            _handler = new CreatePaymentIntentCommandHandler(
                _paymentServiceMock.Object,
                _paymentRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _currentUserServiceMock.Object,
                _userManagerMock.Object,
                _couponServiceMock.Object);
        }

        // ============================================================
        // 1. User is not authenticated
        // ============================================================

        [Fact]
        public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns((string?)null);

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _paymentRepositoryMock.Verify(
                x => x.GetByBookingIdAsync(It.IsAny<int>()),
                Times.Never);

            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(
                    It.IsAny<decimal>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ============================================================
        // 2. Amount <= 0
        // ============================================================

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public async Task Handle_ShouldThrowBadRequest_WhenAmountIsInvalid(
            decimal amount)
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer1");

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: amount,
                CouponCode: null);

            // Act & Assert
            await Assert.ThrowsAsync<Osta.SharedKernel.Exceptions.BadRequestException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _paymentRepositoryMock.Verify(
                x => x.GetByBookingIdAsync(It.IsAny<int>()),
                Times.Never);

            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(
                    It.IsAny<decimal>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ============================================================
        // 3. Create payment without coupon
        // ============================================================

        [Fact]
        public async Task Handle_ShouldCreatePaymentSuccessfully_WhenNoCouponProvided()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _paymentRepositoryMock
                .Setup(x => x.GetByBookingIdAsync(1))
                .ReturnsAsync((Osta.Data.Entities.Payment?)null);

            var paymentIntent = new PaymentIntentResult(
                "client_secret_test",
                "pi_test_123");

            _paymentServiceMock
                .Setup(x => x.CreatePaymentIntentAsync(
                    500,
                    "egp",
                    1,
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(paymentIntent);

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: null);

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("client_secret_test", result.ClientSecret);
            Assert.Equal("pi_test_123", result.PaymentIntentId);

            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(
                    500,
                    "egp",
                    1,
                    null,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _paymentRepositoryMock.Verify(
                x => x.AddAsync(
                    It.Is<Osta.Data.Entities.Payment>(p =>
                        p.BookingId == 1 &&
                        p.Amount == 500 &&
                        p.Status == PaymentStatus.Pending &&
                        p.Method == PaymentMethod.Card &&
                        p.TransactionId == "pi_test_123"),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        // ============================================================
        // 4. Coupon is not found
        // ============================================================

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenCouponDoesNotExist()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(new User
                {
                    Id = userId
                });

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE20",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Coupons?)null);

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: "SAVE20");

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(
                    It.IsAny<decimal>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ============================================================
        // 5. Coupon is inactive
        // ============================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenCouponIsInactive()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(new User
                {
                    Id = userId
                });

            var coupon = new Coupons
            {
                Id = 1,
                Code = "SAVE20",
                IsActive = false,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                UsageLimit = 10,
                UsedCount = 0,
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 20
            };

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE20",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: "SAVE20");

            // Act & Assert
            await Assert.ThrowsAsync<Osta.SharedKernel.Exceptions.BadRequestException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(
                    It.IsAny<decimal>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ============================================================
        // 6. Coupon not started yet
        // ============================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenCouponHasNotStarted()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(new User
                {
                    Id = userId
                });

            var coupon = new Coupons
            {
                Id = 1,
                Code = "SAVE20",
                IsActive = true,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                UsageLimit = 10,
                UsedCount = 0,
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 20
            };

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE20",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: "SAVE20");

            // Act & Assert
            await Assert.ThrowsAsync<Osta.SharedKernel.Exceptions.BadRequestException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        // ============================================================
        // 7. Coupon expired
        // ============================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenCouponIsExpired()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(new User
                {
                    Id = userId
                });

            var coupon = new Coupons
            {
                Id = 1,
                Code = "SAVE20",
                IsActive = true,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                UsageLimit = 10,
                UsedCount = 0,
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 20
            };

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE20",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: "SAVE20");

            // Act & Assert
            await Assert.ThrowsAsync<Osta.SharedKernel.Exceptions.BadRequestException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        // ============================================================
        // 8. Coupon usage limit reached
        // ============================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenCouponUsageLimitReached()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(new User
                {
                    Id = userId
                });

            var coupon = new Coupons
            {
                Id = 1,
                Code = "SAVE20",
                IsActive = true,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                UsageLimit = 5,
                UsedCount = 5,
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 20
            };

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE20",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: "SAVE20");

            // Act & Assert
            await Assert.ThrowsAsync<Osta.SharedKernel.Exceptions.BadRequestException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        // ============================================================
        // 9. User already used coupon
        // ============================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenUserAlreadyUsedCoupon()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(new User
                {
                    Id = userId
                });

            var coupon = new Coupons
            {
                Id = 1,
                Code = "SAVE20",
                IsActive = true,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                UsageLimit = 10,
                UsedCount = 1,
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 20
            };

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE20",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: "SAVE20");

            // Act & Assert
            await Assert.ThrowsAsync<Osta.SharedKernel.Exceptions.BadRequestException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(
                    It.IsAny<decimal>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ============================================================
        // 10. Percentage coupon
        // ============================================================

        [Fact]
        public async Task Handle_ShouldApplyPercentageCouponCorrectly()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(new User
                {
                    Id = userId
                });

            var coupon = new Coupons
            {
                Id = 1,
                Code = "SAVE20",
                IsActive = true,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                UsageLimit = 10,
                UsedCount = 0,
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 20
            };

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE20",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _paymentRepositoryMock
                .Setup(x => x.GetByBookingIdAsync(1))
                .ReturnsAsync((Osta.Data.Entities.Payment?)null);

            _paymentServiceMock
                .Setup(x => x.CreatePaymentIntentAsync(
                    400,
                    "egp",
                    1,
                    "SAVE20",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new PaymentIntentResult(
                        "client_secret",
                        "pi_test"));

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: "SAVE20");

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.Equal("client_secret", result.ClientSecret);
            Assert.Equal("pi_test", result.PaymentIntentId);

            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(
                    400,
                    "egp",
                    1,
                    "SAVE20",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // ============================================================
        // 11. Fixed amount coupon
        // ============================================================

        [Fact]
        public async Task Handle_ShouldApplyFixedCouponCorrectly()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(new User
                {
                    Id = userId
                });

            var coupon = new Coupons
            {
                Id = 2,
                Code = "SAVE100",
                IsActive = true,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                UsageLimit = 10,
                UsedCount = 0,
                DiscountType = DiscountTypeEnum.FixedAmount,
                DiscountValue = 100
            };

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE100",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _paymentRepositoryMock
                .Setup(x => x.GetByBookingIdAsync(1))
                .ReturnsAsync((Osta.Data.Entities.Payment?)null);

            _paymentServiceMock
                .Setup(x => x.CreatePaymentIntentAsync(
                    400,
                    "egp",
                    1,
                    "SAVE100",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new PaymentIntentResult(
                        "client_secret",
                        "pi_test"));

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: "SAVE100");

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.Equal("client_secret", result.ClientSecret);
            Assert.Equal("pi_test", result.PaymentIntentId);

            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(
                    400,
                    "egp",
                    1,
                    "SAVE100",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // ============================================================
        // 12. Existing completed payment
        // ============================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenBookingAlreadyPaid()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var existingPayment = new Osta.Data.Entities.Payment
            {
                Id = 1,
                BookingId = 1,
                Amount = 500,
                Status = PaymentStatus.Completed,
                TransactionId = "pi_old"
            };

            _paymentRepositoryMock
                .Setup(x => x.GetByBookingIdAsync(1))
                .ReturnsAsync(existingPayment);

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: null);

            // Act & Assert
            await Assert.ThrowsAsync<Osta.SharedKernel.Exceptions.BadRequestException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _paymentServiceMock.Verify(
                x => x.UpdatePaymentIntentAmountAsync(
                    It.IsAny<string>(),
                    It.IsAny<decimal>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ============================================================
        // 13. Existing pending payment
        // ============================================================

        [Fact]
        public async Task Handle_ShouldUpdateExistingPayment_WhenPaymentIsPending()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var existingPayment = new Osta.Data.Entities.Payment
            {
                Id = 1,
                BookingId = 1,
                Amount = 500,
                Status = PaymentStatus.Pending,
                TransactionId = "pi_existing"
            };

            _paymentRepositoryMock
                .Setup(x => x.GetByBookingIdAsync(1))
                .ReturnsAsync(existingPayment);

            var paymentIntentResult = new PaymentIntentResult(
                "updated_secret",
                "pi_existing");

            _paymentServiceMock
                .Setup(x => x.UpdatePaymentIntentAmountAsync(
                    "pi_existing",
                    700,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(paymentIntentResult);

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 700,
                CouponCode: null);

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.Equal("updated_secret", result.ClientSecret);
            Assert.Equal("pi_existing", result.PaymentIntentId);

            Assert.Equal(700, existingPayment.Amount);

            _paymentServiceMock.Verify(
                x => x.UpdatePaymentIntentAmountAsync(
                    "pi_existing",
                    700,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _paymentRepositoryMock.Verify(
                x => x.UpdateAsync(
                    existingPayment,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);

            _paymentRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<Osta.Data.Entities.Payment>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ============================================================
        // 14. Coupon + existing pending payment
        // ============================================================

        [Fact]
        public async Task Handle_ShouldApplyCouponAndUpdatePendingPayment()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(new User
                {
                    Id = userId
                });

            var coupon = new Coupons
            {
                Id = 1,
                Code = "SAVE20",
                IsActive = true,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                UsageLimit = 10,
                UsedCount = 0,
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 20
            };

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE20",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var existingPayment = new Osta.Data.Entities.Payment
            {
                Id = 1,
                BookingId = 1,
                Amount = 500,
                Status = PaymentStatus.Pending,
                TransactionId = "pi_existing"
            };

            _paymentRepositoryMock
                .Setup(x => x.GetByBookingIdAsync(1))
                .ReturnsAsync(existingPayment);

            _paymentServiceMock
                .Setup(x => x.UpdatePaymentIntentAmountAsync(
                    "pi_existing",
                    400,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new PaymentIntentResult(
                        "updated_secret",
                        "pi_existing"));

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: "SAVE20");

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.Equal("updated_secret", result.ClientSecret);
            Assert.Equal("pi_existing", result.PaymentIntentId);

            Assert.Equal(400, existingPayment.Amount);

            _paymentServiceMock.Verify(
                x => x.UpdatePaymentIntentAmountAsync(
                    "pi_existing",
                    400,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _paymentRepositoryMock.Verify(
                x => x.UpdateAsync(
                    existingPayment,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        // ============================================================
        // 15. User not found
        // ============================================================

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "customer1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((User?)null);

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: "SAVE20");

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _couponServiceMock.Verify(
                x => x.GetByCodeAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ============================================================
        // 16. CancellationToken is passed correctly
        // ============================================================

        [Fact]
        public async Task Handle_ShouldPassCancellationToken()
        {
            // Arrange
            var userId = "customer1";
            var cancellationToken = new CancellationToken();

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _paymentRepositoryMock
                .Setup(x => x.GetByBookingIdAsync(1))
                .ReturnsAsync((Osta.Data.Entities.Payment?)null);

            _paymentServiceMock
                .Setup(x => x.CreatePaymentIntentAsync(
                    500,
                    "egp",
                    1,
                    null,
                    cancellationToken))
                .ReturnsAsync(
                    new PaymentIntentResult(
                        "client_secret",
                        "pi_test"));

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 500,
                CouponCode: null);

            // Act
            await _handler.Handle(
                command,
                cancellationToken);

            // Assert
            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(
                    500,
                    "egp",
                    1,
                    null,
                    cancellationToken),
                Times.Once);
        }
    }
}

