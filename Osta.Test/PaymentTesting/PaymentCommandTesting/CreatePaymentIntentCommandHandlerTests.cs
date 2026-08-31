
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
using Osta.SharedKernel.Exceptions;
using Osta.SharedKernel.Identity;

namespace Osta.Test.PaymentTesting.PaymentCommandTesting
{
    public class CreatePaymentIntentCommandHandlerTests
    {
        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ICouponService> _couponServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;

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

        // =========================================================
        // Unauthorized
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var command = new CreatePaymentIntentCommand(
                BookingId: 1,
                Amount: 100,
                CouponCode: null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _handler.Handle(command, CancellationToken.None));

            Assert.Equal(
                "User is not authenticated.",
                exception.Message);
        }

        // =========================================================
        // Invalid Amount
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenAmountIsZero()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                1,
                0,
                null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _handler.Handle(command, CancellationToken.None));

            Assert.Equal(
                "Amount must be greater than zero.",
                exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenAmountIsNegative()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                1,
                -100,
                null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _handler.Handle(command, CancellationToken.None));

            Assert.Equal(
                "Amount must be greater than zero.",
                exception.Message);
        }

        // =========================================================
        // Create New Payment
        // =========================================================

        [Fact]
        public async Task Handle_ShouldCreatePayment_WhenNoExistingPayment()
        {
            // Arrange
            var userId = "user1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var command = new CreatePaymentIntentCommand(
                BookingId: 10,
                Amount: 500,
                CouponCode: null);

            _paymentRepositoryMock
                .Setup(x => x.GetByBookingIdAsync(command.BookingId))
                .ReturnsAsync((Data.Entities.Payment?)null);

            var paymentIntent = new PaymentIntentResult(
                "client_secret",
                "pi_123");

            _paymentServiceMock
                .Setup(x => x.CreatePaymentIntentAsync(
                    500,
                    "egp",
                    command.BookingId,
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(paymentIntent);

            _paymentRepositoryMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Data.Entities.Payment>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<Data.Entities.Payment>);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(It.IsAny<int>);

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("client_secret", result.ClientSecret);
            Assert.Equal("pi_123", result.PaymentIntentId);

            _paymentServiceMock.Verify(x =>
                x.CreatePaymentIntentAsync(
                    500,
                    "egp",
                    command.BookingId,
                    null,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _paymentRepositoryMock.Verify(x =>
                x.AddAsync(
                    It.Is<Data.Entities.Payment>(p =>
                        p.BookingId == command.BookingId &&
                        p.Amount == 500 &&
                        p.Status == PaymentStatus.Pending &&
                        p.Method == PaymentMethod.Card &&
                        p.TransactionId == "pi_123"),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        // =========================================================
        // Existing Completed Payment
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenPaymentAlreadyCompleted()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                10,
                500,
                null);

            var existingPayment = new Data.Entities.Payment
            {
                Id = 1,
                BookingId = 10,
                Amount = 500,
                Status = PaymentStatus.Completed,
                TransactionId = "pi_old"
            };

            _paymentRepositoryMock
                .Setup(x => x.GetByBookingIdAsync(10))
                .ReturnsAsync(existingPayment);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _handler.Handle(
                    command,
                    CancellationToken.None));

            Assert.Equal(
                "This booking has already been paid for.",
                exception.Message);

            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(
                    It.IsAny<decimal>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // =========================================================
        // Existing Pending Payment
        // =========================================================

        [Fact]
        public async Task Handle_ShouldUpdateExistingPayment_WhenPaymentIsPending()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                10,
                700,
                null);

            var existingPayment = new Data.Entities.Payment
            {
                Id = 1,
                BookingId = 10,
                Amount = 500,
                Status = PaymentStatus.Pending,
                TransactionId = "pi_existing"
            };

            _paymentRepositoryMock
                .Setup(x => x.GetByBookingIdAsync(10))
                .ReturnsAsync(existingPayment);

            var updatedIntent = new PaymentIntentResult(
                "updated_secret",
                "pi_existing");

            _paymentServiceMock
                .Setup(x => x.UpdatePaymentIntentAmountAsync(
                    "pi_existing",
                    700,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedIntent);

            _paymentRepositoryMock
                .Setup(x => x.UpdateAsync(
                    existingPayment,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(It.IsAny<int>);

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "updated_secret",
                result.ClientSecret);

            Assert.Equal(
                "pi_existing",
                result.PaymentIntentId);

            Assert.Equal(
                700,
                existingPayment.Amount);

            _paymentServiceMock.Verify(x =>
                x.UpdatePaymentIntentAmountAsync(
                    "pi_existing",
                    700,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _paymentRepositoryMock.Verify(x =>
                x.UpdateAsync(
                    existingPayment,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);

            _paymentRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<Data.Entities.Payment>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // =========================================================
        // Coupon - User Not Found
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenCouponUserDoesNotExist()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                1,
                500,
                "SAVE10");

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user1"))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _handler.Handle(
                    command,
                    CancellationToken.None));

            Assert.Equal(
                "User Not Found.",
                exception.Message);
        }

        // =========================================================
        // Coupon Not Found
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenCouponDoesNotExist()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                1,
                500,
                "SAVE10");

            var user = new User
            {
                Id = "user1"
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user1"))
                .ReturnsAsync(user);

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE10",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Coupons?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _handler.Handle(
                    command,
                    CancellationToken.None));

            Assert.Equal(
                "Coupon Not Found.",
                exception.Message);
        }

        // =========================================================
        // Coupon Inactive
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenCouponIsInactive()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                1,
                500,
                "SAVE10");

            var user = new User
            {
                Id = "user1"
            };

            var coupon = CreateValidCoupon();

            coupon.IsActive = false;

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user1"))
                .ReturnsAsync(user);

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE10",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _handler.Handle(
                    command,
                    CancellationToken.None));

            Assert.Equal(
                "Coupon is not active.",
                exception.Message);
        }

        // =========================================================
        // Coupon Not Started
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenCouponHasNotStarted()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                1,
                500,
                "SAVE10");

            var user = new User
            {
                Id = "user1"
            };

            var coupon = CreateValidCoupon();
            coupon.StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user1"))
                .ReturnsAsync(user);

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE10",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _handler.Handle(
                    command,
                    CancellationToken.None));

            Assert.Equal(
                "Coupon is not active yet.",
                exception.Message);
        }

        // =========================================================
        // Coupon Expired
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenCouponIsExpired()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                1,
                500,
                "SAVE10");

            var user = new User
            {
                Id = "user1"
            };

            var coupon = CreateValidCoupon();
            coupon.StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));
            coupon.EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user1"))
                .ReturnsAsync(user);

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE10",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _handler.Handle(
                    command,
                    CancellationToken.None));

            Assert.Equal(
                "Coupon has expired.",
                exception.Message);
        }

        // =========================================================
        // Coupon Usage Limit
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenCouponUsageLimitReached()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                1,
                500,
                "SAVE10");

            var user = new User
            {
                Id = "user1"
            };

            var coupon = CreateValidCoupon();
            coupon.UsageLimit = 10;
            coupon.UsedCount = 10;

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user1"))
                .ReturnsAsync(user);

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE10",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _handler.Handle(
                    command,
                    CancellationToken.None));

            Assert.Equal(
                "Coupon usage limit has been reached.",
                exception.Message);
        }

        // =========================================================
        // User Already Used Coupon
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenUserAlreadyUsedCoupon()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                1,
                500,
                "SAVE10");

            var user = new User
            {
                Id = "user1"
            };

            var coupon = CreateValidCoupon();

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user1"))
                .ReturnsAsync(user);

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE10",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    "user1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _handler.Handle(
                    command,
                    CancellationToken.None));

            Assert.Equal(
                "You have already used this coupon.",
                exception.Message);
        }

        // =========================================================
        // Percentage Coupon
        // =========================================================

        [Fact]
        public async Task Handle_ShouldApplyPercentageCoupon()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                10,
                1000,
                "SAVE10");

            var user = new User
            {
                Id = "user1"
            };

            var coupon = CreateValidCoupon();
            coupon.Id = 5;
            coupon.DiscountType = DiscountTypeEnum.Percentage;
            coupon.DiscountValue = 10;

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user1"))
                .ReturnsAsync(user);

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "SAVE10",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    5,
                    "user1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _paymentRepositoryMock
                .Setup(x => x.GetByBookingIdAsync(10))
                .ReturnsAsync((Data.Entities.Payment?)null);

            var paymentIntent = new PaymentIntentResult(
                "secret",
                "pi_123");

            _paymentServiceMock
                .Setup(x => x.CreatePaymentIntentAsync(
                    900,
                    "egp",
                    10,
                    "SAVE10",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(paymentIntent);

            _paymentRepositoryMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Data.Entities.Payment>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<Data.Entities.Payment>());

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(It.IsAny<int>);

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "pi_123",
                result.PaymentIntentId);

            _paymentServiceMock.Verify(x =>
                x.CreatePaymentIntentAsync(
                    900,
                    "egp",
                    10,
                    "SAVE10",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // =========================================================
        // Fixed Amount Coupon
        // =========================================================

        [Fact]
        public async Task Handle_ShouldApplyFixedAmountCoupon()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                10,
                1000,
                "FIXED100");

            var user = new User
            {
                Id = "user1"
            };

            var coupon = CreateValidCoupon();
            coupon.Id = 10;
            coupon.DiscountType = DiscountTypeEnum.FixedAmount;
            coupon.DiscountValue = 100;

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user1"))
                .ReturnsAsync(user);

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "FIXED100",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    10,
                    "user1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _paymentRepositoryMock
                .Setup(x => x.GetByBookingIdAsync(10))
                .ReturnsAsync((Data.Entities.Payment?)null);

            var paymentIntent = new PaymentIntentResult(
                "secret",
                "pi_fixed");

            _paymentServiceMock
                .Setup(x => x.CreatePaymentIntentAsync(
                    900,
                    "egp",
                    10,
                    "FIXED100",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(paymentIntent);

            _paymentRepositoryMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Data.Entities.Payment>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<Data.Entities.Payment>);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(It.IsAny<int>);

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "pi_fixed",
                result.PaymentIntentId);

            _paymentServiceMock.Verify(x =>
                x.CreatePaymentIntentAsync(
                    900,
                    "egp",
                    10,
                    "FIXED100",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // =========================================================
        // Invalid Percentage
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenPercentageIsInvalid()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                1,
                1000,
                "BAD");

            var user = new User
            {
                Id = "user1"
            };

            var coupon = CreateValidCoupon();
            coupon.DiscountType = DiscountTypeEnum.Percentage;
            coupon.DiscountValue = 150;

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user1"))
                .ReturnsAsync(user);

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "BAD",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    "user1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _handler.Handle(
                    command,
                    CancellationToken.None));

            Assert.Equal(
                "Invalid discount percentage.",
                exception.Message);
        }

        // =========================================================
        // Invalid Fixed Amount
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenFixedDiscountIsInvalid()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                1,
                1000,
                "BAD");

            var user = new User
            {
                Id = "user1"
            };

            var coupon = CreateValidCoupon();
            coupon.DiscountType = DiscountTypeEnum.FixedAmount;
            coupon.DiscountValue = 0;

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user1"))
                .ReturnsAsync(user);

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "BAD",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    "user1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _handler.Handle(
                    command,
                    CancellationToken.None));

            Assert.Equal(
                "Invalid fixed discount amount.",
                exception.Message);
        }

        // =========================================================
        // Discount Cannot Exceed Amount
        // =========================================================

        [Fact]
        public async Task Handle_ShouldNotAllowDiscountGreaterThanAmount()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user1");

            var command = new CreatePaymentIntentCommand(
                1,
                100,
                "BIGDISCOUNT");

            var user = new User
            {
                Id = "user1"
            };

            var coupon = CreateValidCoupon();
            coupon.DiscountType = DiscountTypeEnum.FixedAmount;
            coupon.DiscountValue = 500;

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user1"))
                .ReturnsAsync(user);

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "BIGDISCOUNT",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    "user1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _paymentRepositoryMock
                .Setup(x => x.GetByBookingIdAsync(1))
                .ReturnsAsync((Data.Entities.Payment?)null);

            var paymentIntent = new PaymentIntentResult(
                "secret",
                "pi_big");

            _paymentServiceMock
                .Setup(x => x.CreatePaymentIntentAsync(
                    0,
                    "egp",
                    1,
                    "BIGDISCOUNT",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(paymentIntent);

            _paymentRepositoryMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Data.Entities.Payment>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(It.IsAny<Data.Entities.Payment>);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(It.IsAny<int>);

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                "pi_big",
                result.PaymentIntentId);

            _paymentServiceMock.Verify(x =>
                x.CreatePaymentIntentAsync(
                    0,
                    "egp",
                    1,
                    "BIGDISCOUNT",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // =========================================================
        // Helper
        // =========================================================

        private static Coupons CreateValidCoupon()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return new Coupons
            {
                Id = 1,
                Code = "SAVE10",
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 10,
                StartDate = today.AddDays(-1),
                EndDate = today.AddDays(10),
                UsageLimit = 100,
                UsedCount = 0,
                IsActive = true
            };
        }
    }
}

