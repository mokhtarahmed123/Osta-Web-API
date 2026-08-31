
using Moq;
using Osta.Core.Feature.Coupon.Query.Handler;
using Osta.Core.Feature.Coupon.Query.Model;
using Osta.Domain.Entities.Customer;
using Osta.Domain.Enum;
using Osta.Payment.CouponService;

namespace Osta.Test.CouponTesting.CouponQueryTesting
{
    public class GetAllCouponQueryHandlerTests
    {
        private readonly Mock<ICouponService> _couponServiceMock;
        private readonly GetAllCouponQueryHandler _handler;

        public GetAllCouponQueryHandlerTests()
        {
            _couponServiceMock = new Mock<ICouponService>();

            _handler = new GetAllCouponQueryHandler(
                _couponServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllActiveCouponsSuccessfully()
        {
            // Arrange
            var request = new GetAllCouponQuery(true);

            var coupons = new List<Coupons>
            {
                new Coupons
                {
                    Id = 1,
                    Code = "ACTIVE10",
                    DiscountType = DiscountTypeEnum.Percentage,
                    DiscountValue = 10m,
                    StartDate = new DateOnly(2026, 8, 30),
                    EndDate = new DateOnly(2026, 9, 30),
                    UsageLimit = 100,
                    UsedCount = 10,
                    IsActive = true
                },
                new Coupons
                {
                    Id = 2,
                    Code = "ACTIVE20",
                    DiscountType = DiscountTypeEnum.Percentage,
                    DiscountValue = 20m,
                    StartDate = new DateOnly(2026, 8, 30),
                    EndDate = new DateOnly(2026, 10, 30),
                    UsageLimit = 50,
                    UsedCount = 5,
                    IsActive = true
                }
            };

            _couponServiceMock
                .Setup(x => x.GetAllAsync(
                    true,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupons);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Equal(2, result.Data.Count);

            Assert.Equal(1, result.Data[0].Id);
            Assert.Equal("ACTIVE10", result.Data[0].Code);
            Assert.Equal(DiscountTypeEnum.Percentage, result.Data[0].DiscountType);
            Assert.Equal(10m, result.Data[0].DiscountValue);
            Assert.Equal(new DateOnly(2026, 8, 30), result.Data[0].StartDate);
            Assert.Equal(new DateOnly(2026, 9, 30), result.Data[0].EndDate);
            Assert.Equal(100, result.Data[0].UsageLimit);
            Assert.Equal(10, result.Data[0].UsedCount);
            Assert.True(result.Data[0].IsActive);

            _couponServiceMock.Verify(
                x => x.GetAllAsync(
                    true,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnInactiveCoupons_WhenIsActiveIsFalse()
        {
            // Arrange
            var request = new GetAllCouponQuery(false);

            var coupons = new List<Coupons>
            {
                new Coupons
                {
                    Id = 3,
                    Code = "INACTIVE10",
                    DiscountType = DiscountTypeEnum.FixedAmount,
                    DiscountValue = 25m,
                    StartDate = new DateOnly(2026, 8, 30),
                    EndDate = new DateOnly(2026, 9, 30),
                    UsageLimit = 20,
                    UsedCount = 20,
                    IsActive = false
                }
            };

            _couponServiceMock
                .Setup(x => x.GetAllAsync(
                    false,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupons);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Single(result.Data);

            var coupon = result.Data[0];

            Assert.Equal(3, coupon.Id);
            Assert.Equal("INACTIVE10", coupon.Code);
            Assert.Equal(DiscountTypeEnum.FixedAmount, coupon.DiscountType);
            Assert.Equal(25m, coupon.DiscountValue);
            Assert.False(coupon.IsActive);

            _couponServiceMock.Verify(
                x => x.GetAllAsync(
                    false,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoCouponsFound()
        {
            // Arrange
            var request = new GetAllCouponQuery(true);

            _couponServiceMock
                .Setup(x => x.GetAllAsync(
                    request.IsActive,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Coupons>());

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);

            _couponServiceMock.Verify(
                x => x.GetAllAsync(
                    request.IsActive,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldMapAllCouponPropertiesCorrectly()
        {
            // Arrange
            var request = new GetAllCouponQuery(true);

            var coupon = new Coupons
            {
                Id = 10,
                Code = "SAVE50",
                DiscountType = DiscountTypeEnum.FixedAmount,
                DiscountValue = 50m,
                StartDate = new DateOnly(2026, 9, 1),
                EndDate = new DateOnly(2026, 12, 31),
                UsageLimit = 200,
                UsedCount = 35,
                IsActive = true
            };

            _couponServiceMock
                .Setup(x => x.GetAllAsync(
                    request.IsActive,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Coupons> { coupon });

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            var mappedCoupon = Assert.Single(result.Data);

            Assert.Equal(coupon.Id, mappedCoupon.Id);
            Assert.Equal(coupon.Code, mappedCoupon.Code);
            Assert.Equal(coupon.DiscountType, mappedCoupon.DiscountType);
            Assert.Equal(coupon.DiscountValue, mappedCoupon.DiscountValue);
            Assert.Equal(coupon.StartDate, mappedCoupon.StartDate);
            Assert.Equal(coupon.EndDate, mappedCoupon.EndDate);
            Assert.Equal(coupon.UsageLimit, mappedCoupon.UsageLimit);
            Assert.Equal(coupon.UsedCount, mappedCoupon.UsedCount);
            Assert.Equal(coupon.IsActive, mappedCoupon.IsActive);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToService()
        {
            // Arrange
            var request = new GetAllCouponQuery(true);

            using var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            _couponServiceMock
                .Setup(x => x.GetAllAsync(
                    request.IsActive,
                    cancellationToken))
                .ReturnsAsync(new List<Coupons>());

            // Act
            await _handler.Handle(
                request,
                cancellationToken);

            // Assert
            _couponServiceMock.Verify(
                x => x.GetAllAsync(
                    request.IsActive,
                    cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldRespectIsActiveFilter()
        {
            // Arrange
            var request = new GetAllCouponQuery(false);

            _couponServiceMock
                .Setup(x => x.GetAllAsync(
                    false,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Coupons>());

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _couponServiceMock.Verify(
                x => x.GetAllAsync(
                    false,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _couponServiceMock.Verify(
                x => x.GetAllAsync(
                    true,
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}

