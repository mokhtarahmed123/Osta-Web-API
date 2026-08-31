
using Moq;
using Osta.Core.Feature.Coupon.Command.Handler;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Domain.Entities.Customer;
using Osta.Domain.Enum;
using Osta.Payment.CouponService;

namespace Osta.Test.CouponTesting.CouponCommandTesting
{
    public class AddCouponCommandHandlerTests
    {
        private readonly Mock<ICouponService> _couponServiceMock;
        private readonly AddCouponCommandHandler _handler;

        public AddCouponCommandHandlerTests()
        {
            _couponServiceMock = new Mock<ICouponService>();

            _handler = new AddCouponCommandHandler(
                _couponServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldAddCouponSuccessfully_WhenCodeIsProvided()
        {
            // Arrange
            var request = new AddCouponCommand("summer25", DiscountTypeEnum.Percentage, 25, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(10)), 100)
            {
                Code = "summer25",
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 25,
                StartDate = DateOnly.FromDateTime(DateTime.Now),

                EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                UsageLimit = 100
            };

            Coupons? addedCoupon = null;

            _couponServiceMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Coupons>(),
                    It.IsAny<CancellationToken>()))
                .Callback<Coupons, CancellationToken>((coupon, _) =>
                {
                    addedCoupon = coupon;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            //Assert.Contains(
            //    "Added Successfully",
            //    result.Message);

            Assert.NotNull(addedCoupon);

            Assert.Equal(
                "SUMMER25",
                addedCoupon!.Code);

            Assert.Equal(
                request.DiscountType,
                addedCoupon.DiscountType);

            Assert.Equal(
                request.DiscountValue,
                addedCoupon.DiscountValue);

            Assert.Equal(
                request.StartDate,
                addedCoupon.StartDate);

            Assert.Equal(
                request.EndDate,
                addedCoupon.EndDate);

            Assert.Equal(
                request.UsageLimit,
                addedCoupon.UsageLimit);

            Assert.True(addedCoupon.IsActive);

            _couponServiceMock.Verify(
                x => x.AddAsync(
                    It.IsAny<Coupons>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldGenerateCodeAutomatically_WhenCodeIsNull()
        {
            // Arrange
            var request = new AddCouponCommand(null, DiscountTypeEnum.Percentage, 25, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(10)), 100)
            {
                Code = null,
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 10,
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                UsageLimit = 50
            };

            Coupons? addedCoupon = null;

            _couponServiceMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Coupons>(),
                    It.IsAny<CancellationToken>()))
                .Callback<Coupons, CancellationToken>((coupon, _) =>
                {
                    addedCoupon = coupon;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.NotNull(addedCoupon);

            Assert.False(string.IsNullOrWhiteSpace(
                addedCoupon!.Code));

            Assert.Equal(
                8,
                addedCoupon.Code.Length);

            Assert.Equal(
                addedCoupon.Code,
                addedCoupon.Code.ToUpper());

            _couponServiceMock.Verify(
                x => x.AddAsync(
                    It.Is<Coupons>(c =>
                        !string.IsNullOrWhiteSpace(c.Code) &&
                        c.Code.Length == 8 &&
                        c.IsActive),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldGenerateCodeAutomatically_WhenCodeIsWhiteSpace()
        {
            // Arrange
            var request = new AddCouponCommand("", DiscountTypeEnum.Percentage, 15, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(10)), 100)
            {
                Code = "   ",
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 15,
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                UsageLimit = 20
            };

            Coupons? addedCoupon = null;

            _couponServiceMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Coupons>(),
                    It.IsAny<CancellationToken>()))
                .Callback<Coupons, CancellationToken>((coupon, _) =>
                {
                    addedCoupon = coupon;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(addedCoupon);
            Assert.False(string.IsNullOrWhiteSpace(
                addedCoupon!.Code));

            Assert.Equal(
                8,
                addedCoupon.Code.Length);

            Assert.Equal(
                addedCoupon.Code.ToUpper(),
                addedCoupon.Code);

            _couponServiceMock.Verify(
                x => x.AddAsync(
                    It.Is<Coupons>(c =>
                        c.Code.Length == 8 &&
                        c.IsActive),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToService()
        {
            // Arrange
            var request = new AddCouponCommand("TEST10", DiscountTypeEnum.Percentage, 10, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(10)), 100)
            {
                Code = "TEST10",
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 10,
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                UsageLimit = 10
            };

            using var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            _couponServiceMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Coupons>(),
                    cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                cancellationToken);

            // Assert
            _couponServiceMock.Verify(
                x => x.AddAsync(
                    It.Is<Coupons>(c => c.Code == "TEST10"),
                    cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldSetCouponAsActive()
        {
            // Arrange
            var request = new AddCouponCommand("active10s", DiscountTypeEnum.Percentage, 10, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(10)), 100)
            {
                Code = "active10",
                DiscountType = DiscountTypeEnum.Percentage,
                DiscountValue = 10,
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                UsageLimit = 10
            };

            Coupons? addedCoupon = null;

            _couponServiceMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Coupons>(),
                    It.IsAny<CancellationToken>()))
                .Callback<Coupons, CancellationToken>((coupon, _) =>
                {
                    addedCoupon = coupon;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(addedCoupon);
            Assert.True(addedCoupon!.IsActive);
        }
    }
}

