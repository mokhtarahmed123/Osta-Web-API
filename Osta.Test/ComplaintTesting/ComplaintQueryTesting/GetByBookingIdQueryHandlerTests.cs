
using AutoMapper;
using Moq;
using Osta.Core.Feature.Complaint.Query.Handler;
using Osta.Core.Feature.Complaint.Query.Model;
using Osta.Core.Feature.Complaint.Query.Result;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.ComplaintTesting.ComplaintQueryTesting
{
    public class GetByBookingIdQueryHandlerTests
    {
        private readonly Mock<IComplaintService> _complaintServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly GetByBookingIdQueryHandler _handler;

        public GetByBookingIdQueryHandlerTests()
        {
            _complaintServiceMock = new Mock<IComplaintService>();
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new GetByBookingIdQueryHandler(
                _complaintServiceMock.Object,
                _mapperMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenBookingIdIsZero()
        {
            // Arrange
            var request = new GetByBookingIdQuery(0)
            {
                BookingId = 0
            };

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "Booking ID must be greater than 0.",
                result.Message);

            _complaintServiceMock.Verify(
                x => x.GetByBookingId(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map<List<GetByBookingIdResult>>(
                    It.IsAny<object>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenBookingIdIsNegative()
        {
            // Arrange
            var request = new GetByBookingIdQuery(-1)
            {
                BookingId = -1
            };

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "Booking ID must be greater than 0.",
                result.Message);

            _complaintServiceMock.Verify(
                x => x.GetByBookingId(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnComplaintsSuccessfully()
        {
            // Arrange
            var request = new GetByBookingIdQuery(1)
            {
                BookingId = 1
            };

            var complaints = new List<Data.Entities.Administration.Complaint>
            {
                new Data.Entities.Administration.Complaint
                {
                    Id = 1,
                    BookingId = 1,
                    Description = "Complaint 1"
                },
                new Data.Entities.Administration.Complaint
                {
                    Id = 2,
                    BookingId = 1,
                    Description = "Complaint 2"
                }
            };

            var mappedResult = new List<GetByBookingIdResult>
            {
                new GetByBookingIdResult(),
                new GetByBookingIdResult()
            };

            _complaintServiceMock
                .Setup(x => x.GetByBookingId(
                    request.BookingId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaints);

            _mapperMock
                .Setup(x => x.Map<List<GetByBookingIdResult>>(complaints))
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
                x => x.GetByBookingId(
                    request.BookingId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetByBookingIdResult>>(complaints),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoComplaintsFound()
        {
            // Arrange
            var request = new GetByBookingIdQuery(1)
            {
                BookingId = 1
            };

            var complaints = new List<Data.Entities.Administration.Complaint>();

            var mappedResult = new List<GetByBookingIdResult>();

            _complaintServiceMock
                .Setup(x => x.GetByBookingId(
                    request.BookingId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaints);

            _mapperMock
                .Setup(x => x.Map<List<GetByBookingIdResult>>(complaints))
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
                x => x.GetByBookingId(
                    request.BookingId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetByBookingIdResult>>(complaints),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectBookingIdToService()
        {
            // Arrange
            var request = new GetByBookingIdQuery(25)
            {
                BookingId = 25
            };

            var complaints = new List<Data.Entities.Administration.Complaint>();

            _complaintServiceMock
                .Setup(x => x.GetByBookingId(
                    25,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaints);

            _mapperMock
                .Setup(x => x.Map<List<GetByBookingIdResult>>(complaints))
                .Returns(new List<GetByBookingIdResult>());

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            _complaintServiceMock.Verify(
                x => x.GetByBookingId(
                    25,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
