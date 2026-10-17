
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
    public class GetMediaBookingByTypeQueryHandlerTests
    {
        private readonly Mock<IMediaService> _mediaServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly GetMediaBookingByTypeQueryHandler _handler;

        public GetMediaBookingByTypeQueryHandlerTests()
        {
            _mediaServiceMock = new Mock<IMediaService>();
            _bookingServiceMock = new Mock<IBookingService>();
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new GetMediaBookingByTypeQueryHandler(
                _mediaServiceMock.Object,
                _bookingServiceMock.Object,
                _mapperMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_RequestIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            GetMediaBookingByTypeQuery request = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_BookingIdIsZero_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new GetMediaBookingByTypeQuery(
                0,
                Domain.Enum.RepairMediaTypeEnum.BeforeRepair);

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Succeeded);
            Assert.Equal(
                "Booking Id must be greater than 0.",
                response.Message);
        }

        [Fact]
        public async Task Handle_BookingIdIsNegative_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new GetMediaBookingByTypeQuery(
                -1,
                Domain.Enum.RepairMediaTypeEnum.BeforeRepair);

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Succeeded);
            Assert.Equal(
                "Booking Id must be greater than 0.",
                response.Message);
        }

        [Fact]
        public async Task Handle_UserIdIsNull_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns((string)null);

            var request = new GetMediaBookingByTypeQuery(
                1,
                Domain.Enum.RepairMediaTypeEnum.BeforeRepair);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UserIdIsEmpty_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var request = new GetMediaBookingByTypeQuery(
                1,
                Domain.Enum.RepairMediaTypeEnum.BeforeRepair);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_BookingNotFound_ShouldReturnNotFound()
        {
            // Arrange
            const string userId = "user-1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(1, CancellationToken.None))
                .ReturnsAsync((Bookings)null);

            var request = new GetMediaBookingByTypeQuery(
                1,
                Domain.Enum.RepairMediaTypeEnum.BeforeRepair);

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Succeeded);
            Assert.Equal(
                "Booking not found.",
                response.Message);

            _mediaServiceMock.Verify(
                x => x.GetByTypeAsync(
                    It.IsAny<int>(),
                    It.IsAny<Domain.Enum.RepairMediaTypeEnum>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_UserIsNotCustomerOrTechnician_ShouldReturnUnauthorized()
        {
            // Arrange
            const string userId = "user-1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var booking = new Bookings
            {
                CustomerId = "customer-1",
                TechnicianId = "technician-1"
            };

            _bookingServiceMock
                .Setup(x => x.GetBookingById(1, CancellationToken.None))
                .ReturnsAsync(booking);

            var request = new GetMediaBookingByTypeQuery(
                1,
                Domain.Enum.RepairMediaTypeEnum.BeforeRepair);

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Succeeded);
            Assert.Equal(
                "You are not authorized to access this booking.",
                response.Message);

            _mediaServiceMock.Verify(
                x => x.GetByTypeAsync(
                    It.IsAny<int>(),
                    It.IsAny<Domain.Enum.RepairMediaTypeEnum>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_UserIsCustomer_ShouldReturnNotFound_WhenNoMediaExists()
        {
            // Arrange
            const string userId = "customer-1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var booking = new Bookings
            {
                CustomerId = userId,
                TechnicianId = "technician-1"
            };

            _bookingServiceMock
                .Setup(x => x.GetBookingById(1, CancellationToken.None))
                .ReturnsAsync(booking);

            _mediaServiceMock
                .Setup(x => x.GetByTypeAsync(
                    1,
                    Domain.Enum.RepairMediaTypeEnum.BeforeRepair,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Media>());

            var request = new GetMediaBookingByTypeQuery(
                1,
                Domain.Enum.RepairMediaTypeEnum.BeforeRepair);

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Succeeded);
            Assert.Equal(
                "No media found for this booking and type.",
                response.Message);
        }

        [Fact]
        public async Task Handle_UserIsTechnician_ShouldReturnNotFound_WhenNoMediaExists()
        {
            // Arrange
            const string userId = "technician-1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var booking = new Bookings
            {
                CustomerId = "customer-1",
                TechnicianId = userId
            };

            _bookingServiceMock
                .Setup(x => x.GetBookingById(1, CancellationToken.None))
                .ReturnsAsync(booking);

            _mediaServiceMock
                .Setup(x => x.GetByTypeAsync(
                    1,
                    Domain.Enum.RepairMediaTypeEnum.BeforeRepair,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Media>());

            var request = new GetMediaBookingByTypeQuery(
                1,
                Domain.Enum.RepairMediaTypeEnum.BeforeRepair);

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Succeeded);
            Assert.Equal(
                "No media found for this booking and type.",
                response.Message);
        }

        [Fact]
        public async Task Handle_ValidCustomerAndMediaExists_ShouldReturnSuccess()
        {
            // Arrange
            const string userId = "customer-1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var booking = new Bookings
            {
                CustomerId = userId,
                TechnicianId = "technician-1"
            };

            var media = new List<Media>
            {
                new Media()
            };

            var mappedResult = new List<GetMediaBookingByTypeResult>
            {
                new GetMediaBookingByTypeResult()
            };

            _bookingServiceMock
                .Setup(x => x.GetBookingById(1, CancellationToken.None))
                .ReturnsAsync(booking);

            _mediaServiceMock
                .Setup(x => x.GetByTypeAsync(
                    1,
                    Domain.Enum.RepairMediaTypeEnum.BeforeRepair,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            _mapperMock
                .Setup(x => x.Map<List<GetMediaBookingByTypeResult>>(media))
                .Returns(mappedResult);

            var request = new GetMediaBookingByTypeQuery(
                1,
                Domain.Enum.RepairMediaTypeEnum.BeforeRepair);

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Succeeded);
            Assert.NotNull(response.Data);
            Assert.Single(response.Data);

            _bookingServiceMock.Verify(
                x => x.GetBookingById(1, CancellationToken.None),
                Times.Once);

            _mediaServiceMock.Verify(
                x => x.GetByTypeAsync(
                    1,
                    Domain.Enum.RepairMediaTypeEnum.BeforeRepair,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetMediaBookingByTypeResult>>(media),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ValidTechnicianAndMediaExists_ShouldReturnSuccess()
        {
            // Arrange
            const string userId = "technician-1";

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var booking = new Bookings
            {
                CustomerId = "customer-1",
                TechnicianId = userId
            };

            var media = new List<Media>
            {
                new Media()
            };

            var mappedResult = new List<GetMediaBookingByTypeResult>
            {
                new GetMediaBookingByTypeResult()
            };

            _bookingServiceMock
                .Setup(x => x.GetBookingById(1, CancellationToken.None))
                .ReturnsAsync(booking);

            _mediaServiceMock
                .Setup(x => x.GetByTypeAsync(
                    1,
                    Domain.Enum.RepairMediaTypeEnum.AfterRepair,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            _mapperMock
                .Setup(x => x.Map<List<GetMediaBookingByTypeResult>>(media))
                .Returns(mappedResult);

            var request = new GetMediaBookingByTypeQuery(
                1,
                Domain.Enum.RepairMediaTypeEnum.AfterRepair);

            // Act
            var response = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Succeeded);
            Assert.NotNull(response.Data);
            Assert.Single(response.Data);

            _mediaServiceMock.Verify(
                x => x.GetByTypeAsync(
                    1,
                    Domain.Enum.RepairMediaTypeEnum.AfterRepair,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}

