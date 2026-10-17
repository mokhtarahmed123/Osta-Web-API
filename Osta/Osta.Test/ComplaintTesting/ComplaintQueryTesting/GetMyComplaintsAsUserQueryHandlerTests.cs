
using AutoMapper;
using Moq;
using Osta.Core.Feature.Complaint.Query.Handler;
using Osta.Core.Feature.Complaint.Query.Model;
using Osta.Core.Feature.Complaint.Query.Result;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.ComplaintTesting.ComplaintQueryTesting
{
    public class GetMyComplaintsAsUserQueryHandlerTests
    {
        private readonly Mock<IComplaintService> _complaintServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly GetMyComplaintsAsUserQueryHandler _handler;

        public GetMyComplaintsAsUserQueryHandlerTests()
        {
            _complaintServiceMock = new Mock<IComplaintService>();
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new GetMyComplaintsAsUserQueryHandler(
                _complaintServiceMock.Object,
                _mapperMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIdIsEmpty()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var request = new GetMyComplaintsAsUserQuery();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _handler.Handle(
                    request,
                    CancellationToken.None));

            Assert.Equal(
                "You are not authorized.",
                exception.Message);

            _complaintServiceMock.Verify(
                x => x.GetMyComplaints(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map<List<GetMyComplaintsAsUserResult>>(
                    It.IsAny<IEnumerable<Data.Entities.Administration.Complaint>>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnComplaintsSuccessfully_WhenUserIsAuthenticated()
        {
            // Arrange
            var userId = "user-1";

            var request = new GetMyComplaintsAsUserQuery();

            var complaints = new List<Data.Entities.Administration.Complaint>
            {
                new Data.Entities.Administration.Complaint
                {
                    Id = 1,
                    Description = "Complaint 1"
                },
                new Data.Entities.Administration.Complaint
                {
                    Id = 2,
                    Description = "Complaint 2"
                }
            };

            var mappedResult = new List<GetMyComplaintsAsUserResult>
            {
                new GetMyComplaintsAsUserResult(),
                new GetMyComplaintsAsUserResult()
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _complaintServiceMock
                .Setup(x => x.GetMyComplaints(
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaints);

            _mapperMock
                .Setup(x => x.Map<List<GetMyComplaintsAsUserResult>>(
                    complaints))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Equal(2, result.Data.Count);
            Assert.Same(mappedResult, result.Data);

            _complaintServiceMock.Verify(
                x => x.GetMyComplaints(
                    userId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetMyComplaintsAsUserResult>>(
                    complaints),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenUserHasNoComplaints()
        {
            // Arrange
            var userId = "user-1";

            var request = new GetMyComplaintsAsUserQuery();

            var complaints = new List<Data.Entities.Administration.Complaint>();

            var mappedResult = new List<GetMyComplaintsAsUserResult>();

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _complaintServiceMock
                .Setup(x => x.GetMyComplaints(
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaints);

            _mapperMock
                .Setup(x => x.Map<List<GetMyComplaintsAsUserResult>>(
                    complaints))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Empty(result.Data);

            _complaintServiceMock.Verify(
                x => x.GetMyComplaints(
                    userId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetMyComplaintsAsUserResult>>(
                    complaints),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectUserIdToService()
        {
            // Arrange
            var userId = "customer-123";

            var request = new GetMyComplaintsAsUserQuery();

            var complaints = new List<Data.Entities.Administration.Complaint>();

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _complaintServiceMock
                .Setup(x => x.GetMyComplaints(
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaints);

            _mapperMock
                .Setup(x => x.Map<List<GetMyComplaintsAsUserResult>>(
                    complaints))
                .Returns(new List<GetMyComplaintsAsUserResult>());

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _complaintServiceMock.Verify(
                x => x.GetMyComplaints(
                    userId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToService()
        {
            // Arrange
            var userId = "user-1";

            var request = new GetMyComplaintsAsUserQuery();

            var complaints = new List<Data.Entities.Administration.Complaint>();

            using var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _complaintServiceMock
                .Setup(x => x.GetMyComplaints(
                    userId,
                    cancellationToken))
                .ReturnsAsync(complaints);

            _mapperMock
                .Setup(x => x.Map<List<GetMyComplaintsAsUserResult>>(
                    complaints))
                .Returns(new List<GetMyComplaintsAsUserResult>());

            // Act
            await _handler.Handle(
                request,
                cancellationToken);

            // Assert
            _complaintServiceMock.Verify(
                x => x.GetMyComplaints(
                    userId,
                    cancellationToken),
                Times.Once);
        }
    }
}

