
using Moq;
using Osta.Core.Feature.Coupon.Command.Handler;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Domain.Entities.Customer;
using Osta.Domain.Enum;
using Osta.Payment.CouponService;

namespace Osta.Test.CouponTesting.CouponCommandTesting
{
    public class UpdateCouponCommandHandlerTests
    {
        private readonly Mock<ICouponService> _couponServiceMock;
        private readonly UpdateCouponCommandHandler _handler;

        public UpdateCouponCommandHandlerTests()
        {
            _couponServiceMock = new Mock<ICouponService>();

            _handler = new UpdateCouponCommandHandler(
                _couponServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateCouponSuccessfully()
        {
            // Arrange
            var request = new UpdateCouponCommand(
                Id: 1,
                DiscountType: DiscountTypeEnum.Percentage,
                DiscountValue: 20m,
                StartDate: new DateOnly(2026, 8, 30),
                EndDate: new DateOnly(2026, 9, 30),
                UsageLimit: 100,
                IsActive: true);

            _couponServiceMock
                .Setup(x => x.UpdateAsync(
                    request.Id,
                    It.IsAny<Coupons>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            //Assert.Contains(
            //    "Coupon updated successfully",
            //    result.Message);

            _couponServiceMock.Verify(
                x => x.UpdateAsync(
                    request.Id,
                    It.Is<Coupons>(c =>
                        c.DiscountType == request.DiscountType &&
                        c.DiscountValue == request.DiscountValue &&
                        c.StartDate == request.StartDate &&
                        c.EndDate == request.EndDate &&
                        c.UsageLimit == request.UsageLimit &&
                        c.IsActive == request.IsActive),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUpdateCouponWithFixedAmount()
        {
            // Arrange
            var request = new UpdateCouponCommand(
                Id: 5,
                DiscountType: DiscountTypeEnum.FixedAmount,
                DiscountValue: 50m,
                StartDate: new DateOnly(2026, 9, 1),
                EndDate: new DateOnly(2026, 10, 1),
                UsageLimit: 50,
                IsActive: false);

            _couponServiceMock
                .Setup(x => x.UpdateAsync(
                    request.Id,
                    It.IsAny<Coupons>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            //Assert.Contains(
            //    "Coupon updated successfully",
            //    result.Message);

            _couponServiceMock.Verify(
                x => x.UpdateAsync(
                    5,
                    It.Is<Coupons>(c =>
                        c.DiscountType == DiscountTypeEnum.FixedAmount &&
                        c.DiscountValue == 50m &&
                        c.StartDate == new DateOnly(2026, 9, 1) &&
                        c.EndDate == new DateOnly(2026, 10, 1) &&
                        c.UsageLimit == 50 &&
                        c.IsActive == false),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToService()
        {
            // Arrange
            var request = new UpdateCouponCommand(
                Id: 10,
                DiscountType: DiscountTypeEnum.Percentage,
                DiscountValue: 15m,
                StartDate: new DateOnly(2026, 8, 30),
                EndDate: new DateOnly(2026, 9, 30),
                UsageLimit: 20,
                IsActive: true);

            using var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            _couponServiceMock
                .Setup(x => x.UpdateAsync(
                    request.Id,
                    It.IsAny<Coupons>(),
                    cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                cancellationToken);

            // Assert
            _couponServiceMock.Verify(
                x => x.UpdateAsync(
                    request.Id,
                    It.IsAny<Coupons>(),
                    cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectIdToService()
        {
            // Arrange
            var request = new UpdateCouponCommand(
                Id: 25,
                DiscountType: DiscountTypeEnum.Percentage,
                DiscountValue: 10m,
                StartDate: new DateOnly(2026, 8, 30),
                EndDate: new DateOnly(2026, 9, 30),
                UsageLimit: 100,
                IsActive: true);

            _couponServiceMock
                .Setup(x => x.UpdateAsync(
                    25,
                    It.IsAny<Coupons>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _couponServiceMock.Verify(
                x => x.UpdateAsync(
                    25,
                    It.IsAny<Coupons>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}

