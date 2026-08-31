using Moq;
using Osta.Core.Feature.Coupon.Query.Handler;
using Osta.Core.Feature.Coupon.Query.Model;
using Osta.Domain.Entities.Customer;
using Osta.Domain.Enum;
using Osta.Payment.CouponService;

namespace Osta.Test.CouponTesting.CouponQueryTesting
{
    public class GetCouponByCodeQueryHandlerTests
    {
        private readonly Mock<ICouponService> _couponServiceMock;
        private readonly GetCouponByCodeQueryHandler _handler;

        public GetCouponByCodeQueryHandlerTests()
        {
            _couponServiceMock = new Mock<ICouponService>();

            _handler = new GetCouponByCodeQueryHandler(
                _couponServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCoupon_WhenCouponExists()
        {
            // Arrange
            var request = new GetCouponByCodeQuery("SAVE20");

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
                .Setup(x => x.GetByCodeAsync(
                    request.Code,
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
                x => x.GetByCodeAsync(
                    request.Code,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenCouponDoesNotExist()
        {
            // Arrange
            var request = new GetCouponByCodeQuery("NOTFOUND");

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

            Assert.Null(result.Data);

            _couponServiceMock.Verify(
                x => x.GetByCodeAsync(
                    request.Code,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldMapAllCouponPropertiesCorrectly()
        {
            // Arrange
            var request = new GetCouponByCodeQuery("FIXED50");

            var coupon = new Coupons
            {
                Id = 10,
                Code = "FIXED50",
                DiscountType = DiscountTypeEnum.FixedAmount,
                DiscountValue = 50m,
                StartDate = new DateOnly(2026, 9, 1),
                EndDate = new DateOnly(2026, 12, 31),
                UsageLimit = 200,
                UsedCount = 25,
                IsActive = false
            };

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    request.Code,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            var mappedCoupon = result.Data;

            Assert.NotNull(mappedCoupon);

            Assert.Equal(10, mappedCoupon.Id);
            Assert.Equal("FIXED50", mappedCoupon.Code);
            Assert.Equal(
                DiscountTypeEnum.FixedAmount,
                mappedCoupon.DiscountType);
            Assert.Equal(50m, mappedCoupon.DiscountValue);
            Assert.Equal(
                new DateOnly(2026, 9, 1),
                mappedCoupon.StartDate);
            Assert.Equal(
                new DateOnly(2026, 12, 31),
                mappedCoupon.EndDate);
            Assert.Equal(200, mappedCoupon.UsageLimit);
            Assert.Equal(25, mappedCoupon.UsedCount);
            Assert.False(mappedCoupon.IsActive);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToService()
        {
            // Arrange
            var request = new GetCouponByCodeQuery("TOKEN10");

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
                .Setup(x => x.GetByCodeAsync(
                    request.Code,
                    cancellationToken))
                .ReturnsAsync(coupon);

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
        }

        [Fact]
        public async Task Handle_ShouldUseRequestedCode()
        {
            // Arrange
            var request = new GetCouponByCodeQuery("ABC123");

            var coupon = new Coupons
            {
                Id = 20,
                Code = "ABC123",
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 15m,
                StartDate = new DateOnly(2026, 8, 30),
                EndDate = new DateOnly(2026, 10, 30),
                UsageLimit = 100,
                UsedCount = 5,
                IsActive = true
            };

            _couponServiceMock
                .Setup(x => x.GetByCodeAsync(
                    "ABC123",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal("ABC123", result.Data.Code);

            _couponServiceMock.Verify(
                x => x.GetByCodeAsync(
                    "ABC123",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}

