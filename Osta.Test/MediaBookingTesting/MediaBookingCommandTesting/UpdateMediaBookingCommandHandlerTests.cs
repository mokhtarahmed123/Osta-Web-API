using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Booking.Interface;
using Osta.Core.Feature.MediaBooking.Command.Handler;
using Osta.Core.Feature.MediaBooking.Command.Model;
using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Data.Enum;
using Osta.Domain.Enum;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Test.MediaBookingTesting.MediaBookingCommandTesting
{
    public class UpdateMediaBookingCommandHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IMediaService> _mediaServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;

        private readonly UpdateMediaBookingCommandHandler _handler;

        public UpdateMediaBookingCommandHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _mediaServiceMock = new Mock<IMediaService>();
            _bookingServiceMock = new Mock<IBookingService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            var userStoreMock = new Mock<IUserStore<User>>();

            _userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            _handler = new UpdateMediaBookingCommandHandler(
                _mapperMock.Object,
                _loggerMock.Object,
                _mediaServiceMock.Object,
                _bookingServiceMock.Object,
                _currentUserServiceMock.Object,
                _userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Arrange
            UpdateMediaBookingCommand request = null!;

            // Act
            var act = async () =>
                await _handler.Handle(
                    request,
                    CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var request = new UpdateMediaBookingCommand(1);

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
            var request = new UpdateMediaBookingCommand(1);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

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
                "Media with Id 1 was not found.",
                result.Message);

            _bookingServiceMock.Verify(
                x => x.GetBookingById(It.IsAny<int>(), CancellationToken.None),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            // Arrange
            var request = new UpdateMediaBookingCommand(1);

            var media = new Media
            {
                Id = 1,
                BookingId = 10,
                UploadedByUserId = "customer-1"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            _mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(media.BookingId, CancellationToken.None))
                .ReturnsAsync((Bookings?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "Booking with Id 10 was not found.",
                result.Message);

            _mapperMock.Verify(
                x => x.Map(
                    It.IsAny<UpdateMediaBookingCommand>(),
                    It.IsAny<Media>()),
                Times.Never);

            _mediaServiceMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<int>(),
                    It.IsAny<Media>(),
                    It.IsAny<Microsoft.AspNetCore.Http.IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenBookingDoesNotBelongToCurrentUser()
        {
            // Arrange
            var request = new UpdateMediaBookingCommand(1);

            var media = new Media
            {
                Id = 1,
                BookingId = 10,
                UploadedByUserId = "customer-1"
            };

            var booking = new Bookings
            {
                Id = 10,
                CustomerId = "customer-2"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            _mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(media.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "This media does not belong to you.",
                result.Message);

            _mapperMock.Verify(
                x => x.Map(
                    It.IsAny<UpdateMediaBookingCommand>(),
                    It.IsAny<Media>()),
                Times.Never);

            _mediaServiceMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<int>(),
                    It.IsAny<Media>(),
                    It.IsAny<Microsoft.AspNetCore.Http.IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenUpdateThrowsException()
        {
            // Arrange
            var request = new UpdateMediaBookingCommand(1);

            var media = new Media
            {
                Id = 1,
                BookingId = 10,
                UploadedByUserId = "customer-1"
            };

            var booking = new Bookings
            {
                Id = 10,
                CustomerId = "customer-1"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            _mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(media.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            _mapperMock
                .Setup(x => x.Map(
                    request,
                    media));

            _mediaServiceMock
                .Setup(x => x.UpdateAsync(
                    request.Id,
                    media,
                    request.File,
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Upload failed"));

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(
                "Failed to update media: Upload failed",
                result.Message);

            _mapperMock.Verify(
                x => x.Map(
                    request,
                    media),
                Times.Once);

            _mediaServiceMock.Verify(
                x => x.UpdateAsync(
                    request.Id,
                    media,
                    request.File,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUpdateMediaAndReturnSuccess_WhenRequestIsValid()
        {
            // Arrange
            var request = new UpdateMediaBookingCommand(1)
            {
                File = null!,
                FileType = MediaFileType.Image,
                RepairMediaType = RepairMediaTypeEnum.BeforeRepair,
                Description = "Updated description"
            };

            var media = new Media
            {
                Id = 1,
                BookingId = 10,
                UploadedByUserId = "customer-1"
            };

            var booking = new Bookings
            {
                Id = 10,
                CustomerId = "customer-1"
            };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            _mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(media.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            _mapperMock
                .Setup(x => x.Map(
                    request,
                    media));

            _mediaServiceMock
                .Setup(x => x.UpdateAsync(
                    request.Id,
                    media,
                    request.File,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            //Assert.Contains(
            //    "Media updated successfully.",
            //    result.Message);

            _mapperMock.Verify(
                x => x.Map(
                    request,
                    media),
                Times.Once);

            _mediaServiceMock.Verify(
                x => x.UpdateAsync(
                    request.Id,
                    media,
                    request.File,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}