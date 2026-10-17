using AutoMapper;
using Moq;
using Osta.Core.Feature.Technician.Query.Handler.TechnicianPayoutQueryHandler;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianPayout;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout;
using Osta.Domain.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.TechnicianPayoutTesting.TechnicianPayoutQueryTesting
{
    public class GetAllMyPayoutsQueryHandlerTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ITechnicianPayoutService> _technicianPayoutServiceMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly GetAllMyPayoutsQueryHandler _handler;

        public GetAllMyPayoutsQueryHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _technicianPayoutServiceMock = new Mock<ITechnicianPayoutService>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetAllMyPayoutsQueryHandler(
                _currentUserServiceMock.Object,
                _technicianPayoutServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns((string)null);

            var request = new GetAllMyPayoutsQuery();

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "User is not authenticated.",
                result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.GetTechnicianPayoutsAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map<List<GetAllMyPayoutsResult>>(
                    It.IsAny<object>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIdIsEmpty()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var request = new GetAllMyPayoutsQuery();

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "User is not authenticated.",
                result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.GetTechnicianPayoutsAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIdIsWhitespace()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("   ");

            var request = new GetAllMyPayoutsQuery();

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "User is not authenticated.",
                result.Message);

            _technicianPayoutServiceMock.Verify(
                x => x.GetTechnicianPayoutsAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenPayoutsExist()
        {
            // Arrange
            var technicianId = "tech-123";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var payouts = new List<TechnicianPayout>
            {
                new TechnicianPayout
                {
                    Id = 1,
                    TechnicianId = technicianId,
                    Amount = 100,
                    Status = Domain.Enum.PayoutStatus.Pending
                },
                new TechnicianPayout
                {
                    Id = 2,
                    TechnicianId = technicianId,
                    Amount = 200,
                    Status = Domain.Enum.PayoutStatus.Completed
                }
            };

            var mappedResult = new List<GetAllMyPayoutsResult>
            {
                new GetAllMyPayoutsResult()
            };

            _technicianPayoutServiceMock
                .Setup(x => x.GetTechnicianPayoutsAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payouts);

            _mapperMock
                .Setup(x => x.Map<List<GetAllMyPayoutsResult>>(payouts))
                .Returns(mappedResult);

            var request = new GetAllMyPayoutsQuery();

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                mappedResult,
                result.Data);

            _technicianPayoutServiceMock.Verify(
                x => x.GetTechnicianPayoutsAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetAllMyPayoutsResult>>(payouts),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WithEmptyList_WhenNoPayoutsExist()
        {
            // Arrange
            var technicianId = "tech-123";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var payouts = new List<TechnicianPayout>();

            var mappedResult = new List<GetAllMyPayoutsResult>();

            _technicianPayoutServiceMock
                .Setup(x => x.GetTechnicianPayoutsAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payouts);

            _mapperMock
                .Setup(x => x.Map<List<GetAllMyPayoutsResult>>(payouts))
                .Returns(mappedResult);

            var request = new GetAllMyPayoutsQuery();

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                mappedResult,
                result.Data);

            _technicianPayoutServiceMock.Verify(
                x => x.GetTechnicianPayoutsAsync(
                    technicianId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetAllMyPayoutsResult>>(payouts),
                Times.Once);
        }
    }
}