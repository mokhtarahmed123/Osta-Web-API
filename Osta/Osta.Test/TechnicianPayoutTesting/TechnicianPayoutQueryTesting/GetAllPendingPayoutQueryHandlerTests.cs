using AutoMapper;
using Moq;
using Osta.Core.Feature.Technician.Query.Handler.TechnicianPayoutQueryHandler;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianPayout;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout;
using Osta.Domain.Entities.Technician;
using Osta.Domain.Enum;
using Osta.Service.Abstract.TechnicianAbstract;

namespace Osta.Test.TechnicianPayoutTesting.TechnicianPayoutQueryTesting
{
    public class GetAllPendingPayoutQueryHandlerTests
    {
        private readonly Mock<ITechnicianPayoutService> _technicianPayoutServiceMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly GetAllPendingPayoutQueryHandler _handler;

        public GetAllPendingPayoutQueryHandlerTests()
        {
            _technicianPayoutServiceMock = new Mock<ITechnicianPayoutService>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetAllPendingPayoutQueryHandler(
                _technicianPayoutServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenPendingPayoutsExist()
        {
            // Arrange
            var payouts = new List<TechnicianPayout>
            {
                new TechnicianPayout
                {
                    Id = 1,
                    TechnicianId = "tech-1",
                    Amount = 100,
                    Status = PayoutStatus.Pending
                },
                new TechnicianPayout
                {
                    Id = 2,
                    TechnicianId = "tech-2",
                    Amount = 200,
                    Status = PayoutStatus.Pending
                }
            };

            var mappedResult = new List<GetAllPendingPayoutResult>
            {
                new GetAllPendingPayoutResult(
                    1,
                    "tech-1",
                    100,
                    PayoutStatus.Pending,
                    DateTime.UtcNow),

                new GetAllPendingPayoutResult(
                    2,
                    "tech-2",
                    200,
                    PayoutStatus.Pending,
                    DateTime.UtcNow)
            };

            _technicianPayoutServiceMock
                .Setup(x => x.GetPendingPayoutsAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payouts);

            _mapperMock
                .Setup(x => x.Map<List<GetAllPendingPayoutResult>>(payouts))
                .Returns(mappedResult);

            var request = new GetAllPendingPayoutQuery();

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mappedResult, result.Data);

            Assert.Equal(2, result.Data.Count);

            Assert.Equal(1, result.Data[0].Id);
            Assert.Equal("tech-1", result.Data[0].TechnicianId);
            Assert.Equal(100, result.Data[0].Amount);
            Assert.Equal(PayoutStatus.Pending, result.Data[0].Status);

            _technicianPayoutServiceMock.Verify(
                x => x.GetPendingPayoutsAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetAllPendingPayoutResult>>(payouts),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WithEmptyList_WhenNoPendingPayoutsExist()
        {
            // Arrange
            var payouts = new List<TechnicianPayout>();

            var mappedResult = new List<GetAllPendingPayoutResult>();

            _technicianPayoutServiceMock
                .Setup(x => x.GetPendingPayoutsAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payouts);

            _mapperMock
                .Setup(x => x.Map<List<GetAllPendingPayoutResult>>(payouts))
                .Returns(mappedResult);

            var request = new GetAllPendingPayoutQuery();

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mappedResult, result.Data);
            Assert.Empty(result.Data);

            _technicianPayoutServiceMock.Verify(
                x => x.GetPendingPayoutsAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetAllPendingPayoutResult>>(payouts),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationToken_ToService()
        {
            // Arrange
            var payouts = new List<TechnicianPayout>();

            var mappedResult = new List<GetAllPendingPayoutResult>();

            var cancellationToken = new CancellationTokenSource().Token;

            _technicianPayoutServiceMock
                .Setup(x => x.GetPendingPayoutsAsync(cancellationToken))
                .ReturnsAsync(payouts);

            _mapperMock
                .Setup(x => x.Map<List<GetAllPendingPayoutResult>>(payouts))
                .Returns(mappedResult);

            var request = new GetAllPendingPayoutQuery();

            // Act
            await _handler.Handle(
                request,
                cancellationToken);

            // Assert
            _technicianPayoutServiceMock.Verify(
                x => x.GetPendingPayoutsAsync(cancellationToken),
                Times.Once);
        }
    }
}