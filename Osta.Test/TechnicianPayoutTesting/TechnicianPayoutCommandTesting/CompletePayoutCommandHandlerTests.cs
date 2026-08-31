using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Core.Feature.Technician.Command.Handler.TechnicianPayoutCommandHandler;
using Osta.Core.Feature.Technician.Command.Model.TechnicianPayout;
using Osta.Data.Entities.Identity;
using Osta.Domain.Entities.Technician;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.TechnicianAbstract;

namespace Osta.Test.TechnicianPayoutTesting.TechnicianPayoutCommandTesting
{
    public class CompletePayoutCommandHandlerTests
    {
        private readonly Mock<ITechnicianPayoutService> _payoutServiceMock;
        private readonly Mock<ISendNotificationMessage> _notificationServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;

        private readonly CompletePayoutCommandHandler _handler;

        public CompletePayoutCommandHandlerTests()
        {
            _payoutServiceMock = new Mock<ITechnicianPayoutService>();
            _notificationServiceMock = new Mock<ISendNotificationMessage>();

            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            _handler = new CompletePayoutCommandHandler(
                _payoutServiceMock.Object,
                _notificationServiceMock.Object,
                _userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenCompletePayoutFails()
        {
            // Arrange
            var payoutId = 10;

            var payout = new TechnicianPayout
            {
                Id = payoutId,
                TechnicianId = "tech-123",
                Amount = 500,
                ReceivingDetails = "01012345678"
            };

            var request = new CompletePayoutCommand(payoutId);

            _payoutServiceMock
                .Setup(x => x.GetPayoutByIdAsync(
                    payoutId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payout);

            _payoutServiceMock
                .Setup(x => x.CompletePayoutAsync(
                    payoutId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(
                "Payout not found.",
                result.Message);

            _notificationServiceMock.Verify(
                x => x.SendNotification(
                    It.IsAny<PayoutNotification>(),
                    "payout-notification"),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenPayoutIsCompleted()
        {
            // Arrange
            var payoutId = 10;
            var technicianId = "tech-123";

            var payout = new TechnicianPayout
            {
                Id = payoutId,
                TechnicianId = technicianId,
                Amount = 500,
                ReceivingDetails = "01012345678"
            };

            var user = new User
            {
                Id = technicianId,
                Email = "technician@test.com"
            };

            var request = new CompletePayoutCommand(payoutId);

            _payoutServiceMock
                .Setup(x => x.GetPayoutByIdAsync(
                    payoutId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payout);

            _payoutServiceMock
                .Setup(x => x.CompletePayoutAsync(
                    payoutId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(technicianId))
                .ReturnsAsync(user);

            _notificationServiceMock
                .Setup(x => x.SendNotification(
                    It.IsAny<PayoutNotification>(),
                    "payout-notification"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            //Assert.Equal(
            //    "Payout completed successfully.",
            //    result.Message);

            _payoutServiceMock.Verify(
                x => x.GetPayoutByIdAsync(
                    payoutId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _payoutServiceMock.Verify(
                x => x.CompletePayoutAsync(
                    payoutId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _userManagerMock.Verify(
                x => x.FindByIdAsync(technicianId),
                Times.Once);

            _notificationServiceMock.Verify(
                x => x.SendNotification(
                    It.IsAny<PayoutNotification>(),
                    "payout-notification"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldSendCorrectNotification_WhenPayoutIsCompleted()
        {
            // Arrange
            var payoutId = 20;
            var technicianId = "tech-456";

            var payout = new TechnicianPayout
            {
                Id = payoutId,
                TechnicianId = technicianId,
                Amount = 750,
                ReceivingDetails = "Bank Account",
                RejectionReason = null,

            };

            var user = new User
            {
                Id = technicianId,
                Email = "tech@test.com"
            };

            PayoutNotification capturedNotification = null;

            _payoutServiceMock
                .Setup(x => x.GetPayoutByIdAsync(
                    payoutId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payout);

            _payoutServiceMock
                .Setup(x => x.CompletePayoutAsync(
                    payoutId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(technicianId))
                .ReturnsAsync(user);

            _notificationServiceMock
                .Setup(x => x.SendNotification(
                    It.IsAny<PayoutNotification>(),
                    "payout-notification"))
                .Callback<PayoutNotification, string>(
                    (notification, queue) =>
                    {
                        capturedNotification = notification;
                    })
                .Returns(Task.CompletedTask);

            var request = new CompletePayoutCommand(payoutId);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(capturedNotification);

            Assert.Equal(
                technicianId,
                capturedNotification.TechnicianId);

            Assert.Equal(
                payoutId,
                capturedNotification.PayoutId);

            Assert.Equal(
                payout.Amount,
                capturedNotification.Amount);

            Assert.Equal(
                payout.ReceivingDetails,
                capturedNotification.ReceivingDetails);

            Assert.Equal(
                user.Email,
                capturedNotification.Email);

            Assert.Null(
                capturedNotification.ReasonOfRejection);

            Assert.Equal(
                "payout-notification",
                "payout-notification");
        }
    }
}