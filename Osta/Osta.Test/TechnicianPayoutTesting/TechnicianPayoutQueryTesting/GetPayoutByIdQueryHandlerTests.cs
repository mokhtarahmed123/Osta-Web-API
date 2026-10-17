using AutoMapper;
using Moq;
using Osta.Core.Feature.Technician.Query.Handler.TechnicianPayoutQueryHandler;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianPayout;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout;
using Osta.Domain.Entities.Technician;
using Osta.Domain.Enum;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.TechnicianPayoutTesting.TechnicianPayoutQueryTesting
{
    public class GetPayoutByIdQueryHandlerTests
    {
        private readonly Mock<ICurrentUserService> currentUserServiceMock;
        private readonly Mock<ITechnicianPayoutService> payoutServiceMock;
        private readonly Mock<IMapper> mapperMock;

        private readonly GetPayoutByIdQueryHandler handler;

        public GetPayoutByIdQueryHandlerTests()
        {
            currentUserServiceMock = new Mock<ICurrentUserService>();
            payoutServiceMock = new Mock<ITechnicianPayoutService>();
            mapperMock = new Mock<IMapper>();

            handler = new GetPayoutByIdQueryHandler(
                currentUserServiceMock.Object,
                payoutServiceMock.Object,
                mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns((string)null);

            var request = new GetPayoutByIdQuery(1);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "User is not authenticated.",
                result.Message);

            payoutServiceMock.Verify(
                x => x.GetPayoutByIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenPayoutIdIsInvalid()
        {
            // Arrange
            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("tech-1");

            var request = new GetPayoutByIdQuery(0);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "Invalid payout id.",
                result.Message);

            payoutServiceMock.Verify(
                x => x.GetPayoutByIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenPayoutDoesNotExist()
        {
            // Arrange
            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("tech-1");

            payoutServiceMock
                .Setup(x => x.GetPayoutByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((TechnicianPayout?)null);

            var request = new GetPayoutByIdQuery(1);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "Payout not found.",
                result.Message);

            mapperMock.Verify(
                x => x.Map<GetPayoutByIdResult>(
                    It.IsAny<TechnicianPayout>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenPayoutBelongsToAnotherTechnician()
        {
            // Arrange
            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("tech-1");

            var payout = new TechnicianPayout
            {
                Id = 1,
                TechnicianId = "tech-2",
                Amount = 500m
            };

            payoutServiceMock
                .Setup(x => x.GetPayoutByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payout);

            var request = new GetPayoutByIdQuery(1);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "You are not allowed to access this payout.",
                result.Message);

            mapperMock.Verify(
                x => x.Map<GetPayoutByIdResult>(
                    It.IsAny<TechnicianPayout>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenPayoutBelongsToCurrentTechnician()
        {
            // Arrange
            const string technicianId = "tech-1";

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var requestedAt = DateTime.UtcNow;

            var payout = new TechnicianPayout
            {
                Id = 1,
                TechnicianId = technicianId,
                Amount = 500m,
                Status = PayoutStatus.Pending,
                RequestedAt = requestedAt,
                CompletedAt = null,
                RejectionReason = null
            };

            var mappedResult = new GetPayoutByIdResult(
                Id: 1,
                Amount: 500m,
                Status: PayoutStatus.Pending,
                RequestedAt: requestedAt,
                CompletedAt: null,
                RejectionReason: null
            );

            payoutServiceMock
                .Setup(x => x.GetPayoutByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payout);

            mapperMock
                .Setup(x => x.Map<GetPayoutByIdResult>(payout))
                .Returns(mappedResult);

            var request = new GetPayoutByIdQuery(1);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            Assert.Equal(
                mappedResult.Id,
                result.Data.Id);

            Assert.Equal(
                mappedResult.Amount,
                result.Data.Amount);

            Assert.Equal(
                mappedResult.Status,
                result.Data.Status);

            Assert.Equal(
                mappedResult.RequestedAt,
                result.Data.RequestedAt);

            Assert.Equal(
                mappedResult.CompletedAt,
                result.Data.CompletedAt);

            Assert.Equal(
                mappedResult.RejectionReason,
                result.Data.RejectionReason);

            payoutServiceMock.Verify(
                x => x.GetPayoutByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mapperMock.Verify(
                x => x.Map<GetPayoutByIdResult>(payout),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenPayoutIsCompleted()
        {
            // Arrange
            const string technicianId = "tech-1";

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var requestedAt = DateTime.UtcNow.AddHours(-2);
            var completedAt = DateTime.UtcNow;

            var payout = new TechnicianPayout
            {
                Id = 10,
                TechnicianId = technicianId,
                Amount = 1000m,
                Status = PayoutStatus.Completed,
                RequestedAt = requestedAt,
                CompletedAt = completedAt,
                RejectionReason = null
            };

            var mappedResult = new GetPayoutByIdResult(
                Id: 10,
                Amount: 1000m,
                Status: PayoutStatus.Completed,
                RequestedAt: requestedAt,
                CompletedAt: completedAt,
                RejectionReason: null
            );

            payoutServiceMock
                .Setup(x => x.GetPayoutByIdAsync(
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payout);

            mapperMock
                .Setup(x => x.Map<GetPayoutByIdResult>(payout))
                .Returns(mappedResult);

            var request = new GetPayoutByIdQuery(10);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            Assert.Equal(10, result.Data.Id);
            Assert.Equal(1000m, result.Data.Amount);
            Assert.Equal(PayoutStatus.Completed, result.Data.Status);
            Assert.Equal(completedAt, result.Data.CompletedAt);
            Assert.Null(result.Data.RejectionReason);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenPayoutIsRejected()
        {
            // Arrange
            const string technicianId = "tech-1";

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var requestedAt = DateTime.UtcNow.AddHours(-3);

            var payout = new TechnicianPayout
            {
                Id = 20,
                TechnicianId = technicianId,
                Amount = 750m,
                Status = PayoutStatus.Rejected,
                RequestedAt = requestedAt,
                CompletedAt = null,
                RejectionReason = "Invalid receiving details"
            };

            var mappedResult = new GetPayoutByIdResult(
                Id: 20,
                Amount: 750m,
                Status: PayoutStatus.Rejected,
                RequestedAt: requestedAt,
                CompletedAt: null,
                RejectionReason: "Invalid receiving details"
            );

            payoutServiceMock
                .Setup(x => x.GetPayoutByIdAsync(
                    20,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payout);

            mapperMock
                .Setup(x => x.Map<GetPayoutByIdResult>(payout))
                .Returns(mappedResult);

            var request = new GetPayoutByIdQuery(20);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            Assert.Equal(20, result.Data.Id);
            Assert.Equal(750m, result.Data.Amount);
            Assert.Equal(PayoutStatus.Rejected, result.Data.Status);
            Assert.Null(result.Data.CompletedAt);
            Assert.Equal(
                "Invalid receiving details",
                result.Data.RejectionReason);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToService()
        {
            // Arrange
            const string technicianId = "tech-1";

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var payout = new TechnicianPayout
            {
                Id = 5,
                TechnicianId = technicianId,
                Amount = 300m,
                Status = PayoutStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            var mappedResult = new GetPayoutByIdResult(
                5,
                300m,
                PayoutStatus.Pending,
                payout.RequestedAt,
                null,
                null);

            payoutServiceMock
                .Setup(x => x.GetPayoutByIdAsync(
                    5,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payout);

            mapperMock
                .Setup(x => x.Map<GetPayoutByIdResult>(payout))
                .Returns(mappedResult);

            using var cts = new CancellationTokenSource();

            var request = new GetPayoutByIdQuery(5);

            // Act
            await handler.Handle(
                request,
                cts.Token);

            // Assert
            payoutServiceMock.Verify(
                x => x.GetPayoutByIdAsync(
                    5,
                    cts.Token),
                Times.Once);
        }
    }
}