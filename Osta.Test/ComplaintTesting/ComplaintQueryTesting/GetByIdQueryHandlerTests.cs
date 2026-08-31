
using AutoMapper;
using Moq;
using Osta.Core.Feature.Complaint.Query.Handler;
using Osta.Core.Feature.Complaint.Query.Model;
using Osta.Core.Feature.Complaint.Query.Result;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.ComplaintTesting.ComplaintQueryTesting
{
    public class GetByIdQueryHandlerTests
    {
        private readonly Mock<IComplaintService> _complaintServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly GetByIdQueryHandler _handler;

        public GetByIdQueryHandlerTests()
        {
            _complaintServiceMock = new Mock<IComplaintService>();
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new GetByIdQueryHandler(
                _complaintServiceMock.Object,
                _mapperMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenIdIsZero()
        {
            // Arrange
            var request = new GetByIdQuery(0)
            {
                Id = 0
            };

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "Complaint ID must be greater than 0.",
                result.Message);

            _complaintServiceMock.Verify(
                x => x.GetById(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map<GetByIdResult>(
                    It.IsAny<Data.Entities.Administration.Complaint>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenIdIsNegative()
        {
            // Arrange
            var request = new GetByIdQuery(-1)
            {
                Id = -1
            };

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "Complaint ID must be greater than 0.",
                result.Message);

            _complaintServiceMock.Verify(
                x => x.GetById(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenComplaintDoesNotExist()
        {
            // Arrange
            var request = new GetByIdQuery(1)
            {
                Id = 1
            };

            _complaintServiceMock
                .Setup(x => x.GetById(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Data.Entities.Administration.Complaint?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "Complaint not found.",
                result.Message);

            _mapperMock.Verify(
                x => x.Map<GetByIdResult>(
                    It.IsAny<Data.Entities.Administration.Complaint>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnComplaintSuccessfully()
        {
            // Arrange
            var request = new GetByIdQuery(1)
            {
                Id = 1
            };

            var complaint = new Data.Entities.Administration.Complaint
            {
                Id = 1,
                BookingId = 10,
                Description = "Test complaint"
            };

            var mappedResult = new GetByIdResult();

            _complaintServiceMock
                .Setup(x => x.GetById(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaint);

            _mapperMock
                .Setup(x => x.Map<GetByIdResult>(complaint))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Same(mappedResult, result.Data);

            _complaintServiceMock.Verify(
                x => x.GetById(
                    request.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<GetByIdResult>(complaint),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectIdAndCancellationTokenToService()
        {
            // Arrange
            var request = new GetByIdQuery(15)
            {
                Id = 15
            };

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var complaint = new Data.Entities.Administration.Complaint
            {
                Id = 15,
                BookingId = 20,
                Description = "Test complaint"
            };

            var mappedResult = new GetByIdResult();

            _complaintServiceMock
                .Setup(x => x.GetById(
                    request.Id,
                    cancellationToken))
                .ReturnsAsync(complaint);

            _mapperMock
                .Setup(x => x.Map<GetByIdResult>(complaint))
                .Returns(mappedResult);

            // Act
            await _handler.Handle(
                request,
                cancellationToken);

            // Assert
            _complaintServiceMock.Verify(
                x => x.GetById(
                    15,
                    cancellationToken),
                Times.Once);
        }
    }
}

