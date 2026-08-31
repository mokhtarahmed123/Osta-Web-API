using Moq;
using Osta.Core.Feature.Coupon.Query.Handler;
using Osta.Core.Feature.Coupon.Query.Model;
using Osta.Domain.Entities.Customer;
using Osta.Domain.Enum;
using Osta.Payment.CouponService;

namespace Osta.Test.CouponTesting.CouponQueryTesting
{
    public class GetCouponByIdQueryHandlerTests
    {
        private readonly Mock<ICouponService> _couponServiceMock;
        private readonly GetCouponByIdQueryHandler _handler;

        public GetCouponByIdQueryHandlerTests()
        {
            _couponServiceMock = new Mock<ICouponService>();

            _handler = new GetCouponByIdQueryHandler(
                _couponServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCoupon_WhenCouponExists()
        {
            // Arrange
            var request = new GetCouponByIdQuery(1);

            var coupon = new Coupons
            {
                Id = 1,
                Code = "SAVE20",
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 20m,
                StartDate = new DateOnly(2026, 8, 30),
                EndDate = new DateOnly(2026, 9, 30),
                UsageLimit = 100,
                UsedCount = 10,
                IsActive = true
            };

            _couponServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Equal(coupon.Id, result.Data.Id);
            Assert.Equal(coupon.Code, result.Data.Code);
            Assert.Equal(coupon.DiscountType, result.Data.DiscountType);
            Assert.Equal(coupon.DiscountValue, result.Data.DiscountValue);
            Assert.Equal(coupon.StartDate, result.Data.StartDate);
            Assert.Equal(coupon.EndDate, result.Data.EndDate);
            Assert.Equal(coupon.UsageLimit, result.Data.UsageLimit);
            Assert.Equal(coupon.UsedCount, result.Data.UsedCount);
            Assert.Equal(coupon.IsActive, result.Data.IsActive);

            _couponServiceMock.Verify(
                x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenIdIsNegative()
        {
            // Arrange
            var request = new GetCouponByIdQuery(-1);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "Invalid coupon Id.",
                result.Message);

            Assert.Null(result.Data);

            _couponServiceMock.Verify(
                x => x.GetByIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenCouponDoesNotExist()
        {
            // Arrange
            var request = new GetCouponByIdQuery(999);

            _couponServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
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

            Assert.Null(result.Data);

            _couponServiceMock.Verify(
                x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldMapAllCouponPropertiesCorrectly()
        {
            // Arrange
            var request = new GetCouponByIdQuery(10);

            var coupon = new Coupons
            {
                Id = 10,
                Code = "FIXED50",
                DiscountType = DiscountTypeEnum.FixedAmount,
                DiscountValue = 50m,
                StartDate = new DateOnly(2026, 9, 1),
                EndDate = new DateOnly(2026, 12, 31),
                UsageLimit = 200,
                UsedCount = 35,
                IsActive = false
            };

            _couponServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);

            Assert.Equal(10, result.Data.Id);
            Assert.Equal("FIXED50", result.Data.Code);
            Assert.Equal(
                DiscountTypeEnum.FixedAmount,
                result.Data.DiscountType);
            Assert.Equal(50m, result.Data.DiscountValue);
            Assert.Equal(
                new DateOnly(2026, 9, 1),
                result.Data.StartDate);
            Assert.Equal(
                new DateOnly(2026, 12, 31),
                result.Data.EndDate);
            Assert.Equal(200, result.Data.UsageLimit);
            Assert.Equal(35, result.Data.UsedCount);
            Assert.False(result.Data.IsActive);
        }

        [Fact]
        public async Task Handle_ShouldAllowZeroId()
        {
            // Arrange
            var request = new GetCouponByIdQuery(0);

            var coupon = new Coupons
            {
                Id = 0,
                Code = "ZEROID",
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 10m,
                StartDate = new DateOnly(2026, 8, 30),
                EndDate = new DateOnly(2026, 9, 30),
                UsageLimit = 10,
                UsedCount = 0,
                IsActive = true
            };

            _couponServiceMock
                .Setup(x => x.GetByIdAsync(
                    0,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(0, result.Data.Id);

            _couponServiceMock.Verify(
                x => x.GetByIdAsync(
                    0,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToService()
        {
            // Arrange
            var request = new GetCouponByIdQuery(5);

            using var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            var coupon = new Coupons
            {
                Id = 5,
                Code = "TOKEN10",
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 10m,
                StartDate = new DateOnly(2026, 8, 30),
                EndDate = new DateOnly(2026, 9, 30),
                UsageLimit = 50,
                UsedCount = 0,
                IsActive = true
            };

            _couponServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    cancellationToken))
                .ReturnsAsync(coupon);

            // Act
            await _handler.Handle(
                request,
                cancellationToken);

            // Assert
            _couponServiceMock.Verify(
                x => x.GetByIdAsync(
                    request.Id,
                    cancellationToken),
                Times.Once);
        }
    }
}

