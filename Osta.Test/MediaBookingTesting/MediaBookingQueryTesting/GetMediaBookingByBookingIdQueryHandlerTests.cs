using AutoMapper;
using Moq;
using Osta.Booking.Interface;
using Osta.Core.Feature.MediaBooking.Query.Handler;
using Osta.Core.Feature.MediaBooking.Query.Model;
using Osta.Core.Feature.MediaBooking.Query.Result;
using Osta.Data.Entities.Booking;
using Osta.SharedKernel.Identity;

namespace Osta.Test.MediaBookingTesting.MediaBookingQueryTesting
{
    public class GetMediaBookingByBookingIdQueryHandlerTests
    {
        private readonly Mock<IMediaService> _mediaServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly GetMediaBookingByBookingIdQueryHandler _handler;

        public GetMediaBookingByBookingIdQueryHandlerTests()
        {
            _mediaServiceMock = new Mock<IMediaService>();
            _bookingServiceMock = new Mock<IBookingService>();
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new GetMediaBookingByBookingIdQueryHandler(
                _mediaServiceMock.Object,
                _bookingServiceMock.Object,
                _mapperMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Arrange
            GetMediaBookingByBookingIdQuery request = null!;

            // Act
            var act = async () =>
                await _handler.Handle(
                    request,
                    CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenBookingIdIsLessThanOrEqualToZero()
        {
            // Arrange
            var request = new GetMediaBookingByBookingIdQuery(0);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "Booking Id must be greater than 0.",
                result.Message);

            _bookingServiceMock.Verify(
                x => x.GetBookingById(It.IsAny<int>(), CancellationToken.None),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var request = new GetMediaBookingByBookingIdQuery(1);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns((string?)null);

            // Act
            var act = async () =>
                await _handler.Handle(
                    request,
                    CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);

            _bookingServiceMock.Verify(
                x => x.GetBookingById(It.IsAny<int>(), CancellationToken.None),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            // Arrange
            var request = new GetMediaBookingByBookingIdQuery(1);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-1");

            _bookingServiceMock
                .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
                .ReturnsAsync((Bookings?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "Booking not found.",
                result.Message);

            _mediaServiceMock.Verify(
                x => x.GetByBookingIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotCustomerOrTechnician()
        {
            // Arrange
            var request = new GetMediaBookingByBookingIdQuery(1);

            var booking = new Bookings
            {
                Id = 1,
                CustomerId = "customer-1",
                TechnicianId = "technician-1"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("other-user");

            _bookingServiceMock
                .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "You are not authorized to access this booking.",
                result.Message);

            _mediaServiceMock.Verify(
                x => x.GetByBookingIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenNoMediaExists()
        {
            // Arrange
            var request = new GetMediaBookingByBookingIdQuery(1);

            var booking = new Bookings
            {
                Id = 1,
                CustomerId = "customer-1",
                TechnicianId = "technician-1"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            _bookingServiceMock
                .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            _mediaServiceMock
                .Setup(x => x.GetByBookingIdAsync(
                    request.BookingId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Media>());

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "No media found for this booking.",
                result.Message);

            _mapperMock.Verify(
                x => x.Map<List<GetMediaBookingByBookingIdResult>>(
                    It.IsAny<object>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserIsCustomerAndMediaExists()
        {
            // Arrange
            var request = new GetMediaBookingByBookingIdQuery(1);

            var booking = new Bookings
            {
                Id = 1,
                CustomerId = "customer-1",
                TechnicianId = "technician-1"
            };

            var media = new List<Media>
            {
                new Media
                {
                    Id = 1,
                    BookingId = 1,
                    UploadedByUserId = "customer-1"
                },
                new Media
                {
                    Id = 2,
                    BookingId = 1,
                    UploadedByUserId = "customer-1"
                }
            };

            var mappedResult = new List<GetMediaBookingByBookingIdResult>
            {
                new GetMediaBookingByBookingIdResult(),
                new GetMediaBookingByBookingIdResult()
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            _bookingServiceMock
                .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            _mediaServiceMock
                .Setup(x => x.GetByBookingIdAsync(
                    request.BookingId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            _mapperMock
                .Setup(x => x.Map<List<GetMediaBookingByBookingIdResult>>(media))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);

            _bookingServiceMock.Verify(
                x => x.GetBookingById(request.BookingId, CancellationToken.None),
                Times.Once);

            _mediaServiceMock.Verify(
                x => x.GetByBookingIdAsync(
                    request.BookingId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetMediaBookingByBookingIdResult>>(media),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserIsTechnicianAndMediaExists()
        {
            // Arrange
            var request = new GetMediaBookingByBookingIdQuery(1);

            var booking = new Bookings
            {
                Id = 1,
                CustomerId = "customer-1",
                TechnicianId = "technician-1"
            };

            var media = new List<Media>
            {
                new Media
                {
                    Id = 1,
                    BookingId = 1,
                    UploadedByUserId = "customer-1"
                }
            };

            var mappedResult = new List<GetMediaBookingByBookingIdResult>
            {
                new GetMediaBookingByBookingIdResult()
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("technician-1");

            _bookingServiceMock
                .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            _mediaServiceMock
                .Setup(x => x.GetByBookingIdAsync(
                    request.BookingId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            _mapperMock
                .Setup(x => x.Map<List<GetMediaBookingByBookingIdResult>>(media))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);

            _mapperMock.Verify(
                x => x.Map<List<GetMediaBookingByBookingIdResult>>(media),
                Times.Once);
        }
    }
}