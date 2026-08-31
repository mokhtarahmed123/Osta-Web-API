
using Moq;
using Osta.Core.Feature.Coupon.Command.Handler;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Domain.Entities.Customer;
using Osta.Domain.Enum;
using Osta.Payment.CouponService;

namespace Osta.Test.CouponTesting.CouponCommandTesting
{
    public class AddRangeCouponsCommandHandlerTests
    {
        private readonly Mock<ICouponService> _couponServiceMock;
        private readonly AddRangeCouponsCommandHandler _handler;

        public AddRangeCouponsCommandHandlerTests()
        {
            _couponServiceMock = new Mock<ICouponService>();

            _handler = new AddRangeCouponsCommandHandler(
                _couponServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldGenerateRequestedNumberOfCoupons()
        {
            // Arrange
            var request = new AddRangeCouponsCommand(
                Count: 5,
                DiscountType: DiscountTypeEnum.Percentage,
                DiscountValue: 20m,
                StartDate: new DateOnly(2026, 8, 30),
                EndDate: new DateOnly(2026, 9, 30),
                UsageLimit: 100);

            List<Coupons>? addedCoupons = null;

            _couponServiceMock
                .Setup(x => x.AddRangeAsync(
                    It.IsAny<List<Coupons>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<List<Coupons>, CancellationToken>((coupons, _) =>
                {
                    addedCoupons = coupons;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Equal(5, result.Data.Count);

            Assert.NotNull(addedCoupons);
            Assert.Equal(5, addedCoupons!.Count);

            _couponServiceMock.Verify(
                x => x.AddRangeAsync(
                    It.IsAny<List<Coupons>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldGenerateUniqueCodes()
        {
            // Arrange
            var request = new AddRangeCouponsCommand(
                Count: 20,
                DiscountType: DiscountTypeEnum.Percentage,
                DiscountValue: 10m,
                StartDate: new DateOnly(2026, 8, 30),
                EndDate: new DateOnly(2026, 9, 30),
                UsageLimit: 50);

            List<Coupons>? addedCoupons = null;

            _couponServiceMock
                .Setup(x => x.AddRangeAsync(
                    It.IsAny<List<Coupons>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<List<Coupons>, CancellationToken>((coupons, _) =>
                {
                    addedCoupons = coupons;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(addedCoupons);

            var codes = addedCoupons!
                .Select(x => x.Code)
                .ToList();

            Assert.Equal(
                codes.Count,
                codes.Distinct().Count());

            Assert.Equal(20, codes.Count);
        }

        [Fact]
        public async Task Handle_ShouldGenerateCodesWithEightUppercaseCharacters()
        {
            // Arrange
            var request = new AddRangeCouponsCommand(
                Count: 10,
                DiscountType: DiscountTypeEnum.FixedAmount,
                DiscountValue: 50m,
                StartDate: new DateOnly(2026, 8, 30),
                EndDate: new DateOnly(2026, 9, 30),
                UsageLimit: 10);

            List<Coupons>? addedCoupons = null;

            _couponServiceMock
                .Setup(x => x.AddRangeAsync(
                    It.IsAny<List<Coupons>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<List<Coupons>, CancellationToken>((coupons, _) =>
                {
                    addedCoupons = coupons;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(addedCoupons);

            foreach (var coupon in addedCoupons!)
            {
                Assert.NotNull(coupon.Code);

                Assert.Equal(8, coupon.Code.Length);

                Assert.Equal(
                    coupon.Code.ToUpperInvariant(),
                    coupon.Code);
            }
        }

        [Fact]
        public async Task Handle_ShouldSetCouponPropertiesCorrectly()
        {
            // Arrange
            var request = new AddRangeCouponsCommand(
                Count: 3,
                DiscountType: DiscountTypeEnum.FixedAmount,
                DiscountValue: 75m,
                StartDate: new DateOnly(2026, 9, 1),
                EndDate: new DateOnly(2026, 10, 1),
                UsageLimit: 25);

            List<Coupons>? addedCoupons = null;

            _couponServiceMock
                .Setup(x => x.AddRangeAsync(
                    It.IsAny<List<Coupons>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<List<Coupons>, CancellationToken>((coupons, _) =>
                {
                    addedCoupons = coupons;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(addedCoupons);
            Assert.Equal(3, addedCoupons!.Count);

            foreach (var coupon in addedCoupons)
            {
                Assert.Equal(
                    request.DiscountType,
                    coupon.DiscountType);

                Assert.Equal(
                    request.DiscountValue,
                    coupon.DiscountValue);

                Assert.Equal(
                    request.StartDate,
                    coupon.StartDate);

                Assert.Equal(
                    request.EndDate,
                    coupon.EndDate);

                Assert.Equal(
                    request.UsageLimit,
                    coupon.UsageLimit);

                Assert.True(coupon.IsActive);
            }
        }

        [Fact]
        public async Task Handle_ShouldReturnGeneratedCodes()
        {
            // Arrange
            var request = new AddRangeCouponsCommand(
                Count: 3,
                DiscountType: DiscountTypeEnum.Percentage,
                DiscountValue: 15m,
                StartDate: new DateOnly(2026, 8, 30),
                EndDate: new DateOnly(2026, 9, 30),
                UsageLimit: 100);

            List<Coupons>? addedCoupons = null;

            _couponServiceMock
                .Setup(x => x.AddRangeAsync(
                    It.IsAny<List<Coupons>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<List<Coupons>, CancellationToken>((coupons, _) =>
                {
                    addedCoupons = coupons;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);
            Assert.NotNull(addedCoupons);

            var generatedCodes = addedCoupons!
                .Select(x => x.Code)
                .ToList();

            Assert.Equal(
                generatedCodes,
                result.Data);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToService()
        {
            // Arrange
            var request = new AddRangeCouponsCommand(
                Count: 2,
                DiscountType: DiscountTypeEnum.Percentage,
                DiscountValue: 10m,
                StartDate: new DateOnly(2026, 8, 30),
                EndDate: new DateOnly(2026, 9, 30),
                UsageLimit: 10);

            using var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            _couponServiceMock
                .Setup(x => x.AddRangeAsync(
                    It.IsAny<List<Coupons>>(),
                    cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                cancellationToken);

            // Assert
            _couponServiceMock.Verify(
                x => x.AddRangeAsync(
                    It.Is<List<Coupons>>(coupons =>
                        coupons.Count == request.Count),
                    cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCreateNoCoupons_WhenCountIsZero()
        {
            // Arrange
            var request = new AddRangeCouponsCommand(
                Count: 0,
                DiscountType: DiscountTypeEnum.Percentage,
                DiscountValue: 10m,
                StartDate: new DateOnly(2026, 8, 30),
                EndDate: new DateOnly(2026, 9, 30),
                UsageLimit: 10);

            List<Coupons>? addedCoupons = null;

            _couponServiceMock
                .Setup(x => x.AddRangeAsync(
                    It.IsAny<List<Coupons>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<List<Coupons>, CancellationToken>((coupons, _) =>
                {
                    addedCoupons = coupons;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);

            Assert.NotNull(addedCoupons);
            Assert.Empty(addedCoupons!);

            _couponServiceMock.Verify(
                x => x.AddRangeAsync(
                    It.IsAny<List<Coupons>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}

