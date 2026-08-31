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
    public class GetMediaBookingByIdQueryHandlerTests
    {
        private readonly Mock<IMediaService> _mediaServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly GetMediaBookingByIdQueryHandler _handler;

        public GetMediaBookingByIdQueryHandlerTests()
        {
            _mediaServiceMock = new Mock<IMediaService>();
            _bookingServiceMock = new Mock<IBookingService>();
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new GetMediaBookingByIdQueryHandler(
                _mediaServiceMock.Object,
                _bookingServiceMock.Object,
                _mapperMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Arrange
            GetMediaBookingByIdQuery request = null!;

            // Act
            var act = async () =>
                await _handler.Handle(
                    request,
                    CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Handle_ShouldReturnBadRequest_WhenIdIsLessThanOrEqualToZero(
            int id)
        {
            // Arrange
            var request = new GetMediaBookingByIdQuery(id);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "Media Id must be greater than 0.",
                result.Message);

            _mediaServiceMock.Verify(
                x => x.GetByIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var request = new GetMediaBookingByIdQuery(1);

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

            _mediaServiceMock.Verify(
                x => x.GetByIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenMediaDoesNotExist()
        {
            // Arrange
            var request = new GetMediaBookingByIdQuery(1);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-1");

            _mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Media?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "Media not found.",
                result.Message);

            _bookingServiceMock.Verify(
                x => x.GetBookingById(
                    It.IsAny<int>(), CancellationToken.None),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            // Arrange
            var request = new GetMediaBookingByIdQuery(1);

            var media = new Media
            {
                Id = 1,
                BookingId = 10,
                UploadedByUserId = "user-1"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-1");

            _mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    media.BookingId, CancellationToken.None))
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

            _mapperMock.Verify(
                x => x.Map<GetMediaBookingByIdResult>(
                    It.IsAny<Media>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotCustomerOrTechnician()
        {
            // Arrange
            var request = new GetMediaBookingByIdQuery(1);

            var media = new Media
            {
                Id = 1,
                BookingId = 10,
                UploadedByUserId = "customer-1"
            };

            var booking = new Bookings
            {
                Id = 10,
                CustomerId = "customer-1",
                TechnicianId = "technician-1"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("other-user");

            _mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    media.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "You are not authorized to access this media.",
                result.Message);

            _mapperMock.Verify(
                x => x.Map<GetMediaBookingByIdResult>(
                    It.IsAny<Media>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserIsCustomer()
        {
            // Arrange
            var request = new GetMediaBookingByIdQuery(1);

            var media = new Media
            {
                Id = 1,
                BookingId = 10,
                UploadedByUserId = "customer-1"
            };

            var booking = new Bookings
            {
                Id = 10,
                CustomerId = "customer-1",
                TechnicianId = "technician-1"
            };

            var mappedResult = new GetMediaBookingByIdResult();

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            _mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    media.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            _mapperMock
                .Setup(x => x.Map<GetMediaBookingByIdResult>(
                    media))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            _mediaServiceMock.Verify(
                x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _bookingServiceMock.Verify(
                x => x.GetBookingById(
                    media.BookingId, CancellationToken.None),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<GetMediaBookingByIdResult>(
                    media),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserIsTechnician()
        {
            // Arrange
            var request = new GetMediaBookingByIdQuery(1);

            var media = new Media
            {
                Id = 1,
                BookingId = 10,
                UploadedByUserId = "customer-1"
            };

            var booking = new Bookings
            {
                Id = 10,
                CustomerId = "customer-1",
                TechnicianId = "technician-1"
            };

            var mappedResult = new GetMediaBookingByIdResult();

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("technician-1");

            _mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    media.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            _mapperMock
                .Setup(x => x.Map<GetMediaBookingByIdResult>(
                    media))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            _mapperMock.Verify(
                x => x.Map<GetMediaBookingByIdResult>(
                    media),
                Times.Once);
        }
    }
}