
using Moq;
using Osta.Core.Feature.Coupon.Command.Handler;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Domain.Entities.Customer;
using Osta.Payment.CouponService;

namespace Osta.Test.CouponTesting.CouponCommandTesting
{
    public class DeleteCouponCommandHandlerTests
    {
        private readonly Mock<ICouponService> _couponServiceMock;
        private readonly DeleteCouponCommandHandler _handler;

        public DeleteCouponCommandHandlerTests()
        {
            _couponServiceMock = new Mock<ICouponService>();

            _handler = new DeleteCouponCommandHandler(
                _couponServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenIdIsNegative()
        {
            // Arrange
            var request = new DeleteCouponCommand(-1);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "Invalid coupon Id.",
                result.Message);

            _couponServiceMock.Verify(
                x => x.GetByIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _couponServiceMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<int>(),
                    It.IsAny<Coupons>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenCouponDoesNotExist()
        {
            // Arrange
            var request = new DeleteCouponCommand(1);

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

            _couponServiceMock.Verify(
                x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _couponServiceMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<int>(),
                    It.IsAny<Coupons>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenCouponIsAlreadyDeactivated()
        {
            // Arrange
            var request = new DeleteCouponCommand(1);

            var coupon = new Coupons
            {
                Id = request.Id,
                Code = "TEST10",
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
            Assert.NotNull(result);

            Assert.Contains(
                "Coupon is already deactivated.",
                result.Message);

            _couponServiceMock.Verify(
                x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _couponServiceMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<int>(),
                    It.IsAny<Coupons>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldDeactivateCouponSuccessfully()
        {
            // Arrange
            var request = new DeleteCouponCommand(1);

            var coupon = new Coupons
            {
                Id = request.Id,
                Code = "TEST10",
                IsActive = true
            };

            _couponServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

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
            //    "Coupon deactivated successfully",
            //    result.Message);

            Assert.False(coupon.IsActive);

            _couponServiceMock.Verify(
                x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _couponServiceMock.Verify(
                x => x.UpdateAsync(
                    request.Id,
                    It.Is<Coupons>(c => !c.IsActive),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToServices()
        {
            // Arrange
            var request = new DeleteCouponCommand(10);

            var coupon = new Coupons
            {
                Id = request.Id,
                Code = "TOKEN10",
                IsActive = true
            };

            using var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            _couponServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    cancellationToken))
                .ReturnsAsync(coupon);

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
                x => x.GetByIdAsync(
                    request.Id,
                    cancellationToken),
                Times.Once);

            _couponServiceMock.Verify(
                x => x.UpdateAsync(
                    request.Id,
                    It.Is<Coupons>(c => !c.IsActive),
                    cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotModifyCoupon_WhenCouponIsAlreadyInactive()
        {
            // Arrange
            var request = new DeleteCouponCommand(5);

            var coupon = new Coupons
            {
                Id = request.Id,
                Code = "INACTIVE",
                IsActive = false
            };

            _couponServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(coupon.IsActive);

            _couponServiceMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<int>(),
                    It.IsAny<Coupons>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}

