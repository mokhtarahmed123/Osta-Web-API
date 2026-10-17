
using Moq;
using Osta.Core.Feature.Coupon.Command.Handler;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Domain.Entities.Customer;
using Osta.Domain.Enum;
using Osta.Payment.CouponService;

namespace Osta.Test.CouponTesting.CouponCommandTesting
{
    public class ApplyCouponCommandHandlerTests
    {
        private readonly Mock<ICouponService> _couponServiceMock;
        private readonly ApplyCouponCommandHandler _handler;

        public ApplyCouponCommandHandlerTests()
        {
            _couponServiceMock = new Mock<ICouponService>();

            _handler = new ApplyCouponCommandHandler(
                _couponServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenCouponDoesNotExist()
        {
            // Arrange
            var request = new ApplyCouponCommand(
                "NOTFOUND",
                "user-1",
                100m);

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    request.Code,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Coupons?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "Coupon not found.",
                result.Message);

            _couponServiceMock.Verify(
                x => x.GetByCodeAsync(
                    request.Code,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _couponServiceMock.Verify(
                x => x.HasUserUsedCouponAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenCouponIsInactive()
        {
            // Arrange
            var request = new ApplyCouponCommand(
                "TEST10",
                "user-1",
                100m);

            var coupon = CreateValidCoupon();

            coupon.IsActive = false;

            SetupCoupon(request, coupon);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Contains(
                "Coupon is inactive.",
                result.Message);

            _couponServiceMock.Verify(
                x => x.HasUserUsedCouponAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenCouponIsNotValidAtCurrentDate()
        {
            // Arrange
            var request = new ApplyCouponCommand(
                "EXPIRED",
                "user-1",
                100m);

            var coupon = CreateValidCoupon();

            coupon.StartDate = DateOnly.FromDateTime(
                DateTime.UtcNow.AddDays(-10));

            coupon.EndDate = DateOnly.FromDateTime(
                DateTime.UtcNow.AddDays(-1));

            SetupCoupon(request, coupon);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Contains(
                "Coupon is not valid at this time.",
                result.Message);

            _couponServiceMock.Verify(
                x => x.HasUserUsedCouponAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenCouponUsageLimitIsReached()
        {
            // Arrange
            var request = new ApplyCouponCommand(
                "LIMIT10",
                "user-1",
                100m);

            var coupon = CreateValidCoupon();

            coupon.UsedCount = coupon.UsageLimit;

            SetupCoupon(request, coupon);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Contains(
                "Coupon usage limit reached.",
                result.Message);

            _couponServiceMock.Verify(
                x => x.HasUserUsedCouponAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenUserAlreadyUsedCoupon()
        {
            // Arrange
            var request = new ApplyCouponCommand(
                "USED10",
                "user-1",
                100m);

            var coupon = CreateValidCoupon();

            SetupCoupon(request, coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    request.UserId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Contains(
                "You have already used this coupon.",
                result.Message);

            _couponServiceMock.Verify(
                x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    request.UserId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldApplyPercentageDiscountSuccessfully()
        {
            // Arrange
            var request = new ApplyCouponCommand(
                "PERCENT20",
                "user-1",
                100m);

            var coupon = CreateValidCoupon();

            coupon.DiscountType = DiscountTypeEnum.Percentage;
            coupon.DiscountValue = 20;

            SetupCoupon(request, coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    request.UserId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Equal(coupon.Id, result.Data.CouponId);
            Assert.Equal(100m, result.Data.OriginalAmount);
            Assert.Equal(20m, result.Data.DiscountApplied);
            Assert.Equal(80m, result.Data.FinalAmount);
        }

        [Fact]
        public async Task Handle_ShouldApplyFixedAmountDiscountSuccessfully()
        {
            // Arrange
            var request = new ApplyCouponCommand(
                "FIXED20",
                "user-1",
                100m);

            var coupon = CreateValidCoupon();

            coupon.DiscountType = DiscountTypeEnum.FixedAmount;
            coupon.DiscountValue = 20;

            SetupCoupon(request, coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    request.UserId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Equal(coupon.Id, result.Data.CouponId);
            Assert.Equal(100m, result.Data.OriginalAmount);
            Assert.Equal(20m, result.Data.DiscountApplied);
            Assert.Equal(80m, result.Data.FinalAmount);
        }

        [Fact]
        public async Task Handle_ShouldNotAllowDiscountGreaterThanOriginalAmount()
        {
            // Arrange
            var request = new ApplyCouponCommand(
                "BIGDISCOUNT",
                "user-1",
                50m);

            var coupon = CreateValidCoupon();

            coupon.DiscountType = DiscountTypeEnum.FixedAmount;
            coupon.DiscountValue = 100;

            SetupCoupon(request, coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    request.UserId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);

            Assert.Equal(50m, result.Data.OriginalAmount);
            Assert.Equal(50m, result.Data.DiscountApplied);
            Assert.Equal(0m, result.Data.FinalAmount);
        }

        [Fact]
        public async Task Handle_ShouldReturnZeroDiscount_WhenDiscountTypeIsUnknown()
        {
            // Arrange
            var request = new ApplyCouponCommand(
                "UNKNOWN",
                "user-1",
                100m);

            var coupon = CreateValidCoupon();

            // Use an enum value that is not Percentage or FixedAmount
            coupon.DiscountType = (DiscountTypeEnum)999;
            coupon.DiscountValue = 50;

            SetupCoupon(request, coupon);

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    request.UserId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);

            Assert.Equal(0m, result.Data.DiscountApplied);
            Assert.Equal(100m, result.Data.FinalAmount);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectCancellationToken()
        {
            // Arrange
            var request = new ApplyCouponCommand(
                "TEST10",
                "user-1",
                100m);

            var coupon = CreateValidCoupon();

            SetupCoupon(request, coupon);

            using var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            _couponServiceMock
                .Setup(x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    request.UserId,
                    cancellationToken))
                .ReturnsAsync(false);

            // Act
            await _handler.Handle(
                request,
                cancellationToken);

            // Assert
            _couponServiceMock.Verify(
                x => x.GetByCodeAsync(
                    request.Code,
                    cancellationToken),
                Times.Once);

            _couponServiceMock.Verify(
                x => x.HasUserUsedCouponAsync(
                    coupon.Id,
                    request.UserId,
                    cancellationToken),
                Times.Once);
        }

        private void SetupCoupon(
            ApplyCouponCommand request,
            Coupons coupon)
        {
            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    request.Code,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);
        }

        private static Coupons CreateValidCoupon()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return new Coupons
            {
                Id = 1,
                Code = "TEST10",
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

