using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    public class AddMediaBookingCommandHandlerTests
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly Mock<IMediaService> mediaServiceMock;
        private readonly Mock<IBookingService> bookingServiceMock;
        private readonly Mock<ICurrentUserService> currentUserServiceMock;
        private readonly Mock<UserManager<User>> userManagerMock;

        private readonly AddMediaBookingCommandHandler handler;

        public AddMediaBookingCommandHandlerTests()
        {
            mapperMock = new Mock<IMapper>();
            loggerMock = new Mock<ILoggerService>();
            mediaServiceMock = new Mock<IMediaService>();
            bookingServiceMock = new Mock<IBookingService>();
            currentUserServiceMock = new Mock<ICurrentUserService>();

            var userStoreMock = new Mock<IUserStore<User>>();

            userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);

            handler = new AddMediaBookingCommandHandler(
                mapperMock.Object,
                loggerMock.Object,
                mediaServiceMock.Object,
                bookingServiceMock.Object,
                currentUserServiceMock.Object,
                userManagerMock.Object);
        }

        // ---------------------------------------------------------
        // Helper
        // ---------------------------------------------------------

        private static IFormFile CreateMockFile(
            string fileName = "test.jpg",
            long length = 100)
        {
            var fileMock = new Mock<IFormFile>();

            fileMock.Setup(f => f.Length)
                .Returns(length);

            fileMock.Setup(f => f.FileName)
                .Returns(fileName);

            fileMock.Setup(f => f.ContentType)
                .Returns("image/jpeg");

            return fileMock.Object;
        }

        private static AddMediaBookingCommand CreateValidRequest()
        {
            return new AddMediaBookingCommand
            {
                BookingId = 1,
                File = CreateMockFile(),
                FileType = MediaFileType.Image,
                RepairMediaType = RepairMediaTypeEnum.BeforeRepair,
                Description = "Test image"
            };
        }

        // ---------------------------------------------------------
        // 1. Request is null
        // ---------------------------------------------------------

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Arrange
            AddMediaBookingCommand request = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => handler.Handle(request, CancellationToken.None));
        }

        // ---------------------------------------------------------
        // 2. User is not authenticated
        // ---------------------------------------------------------

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIdIsNull()
        {
            // Arrange
            var request = CreateValidRequest();

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns((string?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(request, CancellationToken.None));

            bookingServiceMock.Verify(
                x => x.GetBookingById(It.IsAny<int>(), CancellationToken.None),
                Times.Never);

            mediaServiceMock.Verify(
                x => x.AddAsync(
                    It.IsAny<Media>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ---------------------------------------------------------
        // 3. UserId is empty
        // ---------------------------------------------------------

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIdIsEmpty()
        {
            // Arrange
            var request = CreateValidRequest();

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(request, CancellationToken.None));
        }

        // ---------------------------------------------------------
        // 4. File is null
        // ---------------------------------------------------------

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenFileIsNull()
        {
            // Arrange
            var request = CreateValidRequest();
            request.File = null!;

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            // Act
            var response = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);

            mediaServiceMock.Verify(
                x => x.AddAsync(
                    It.IsAny<Media>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ---------------------------------------------------------
        // 5. File length is zero
        // ---------------------------------------------------------

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenFileIsEmpty()
        {
            // Arrange
            var request = CreateValidRequest();

            request.File = CreateMockFile(
                "empty.jpg",
                0);

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            // Act
            var response = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);

            bookingServiceMock.Verify(
                x => x.GetBookingById(It.IsAny<int>(), CancellationToken.None),
                Times.Never);

            mediaServiceMock.Verify(
                x => x.AddAsync(
                    It.IsAny<Media>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ---------------------------------------------------------
        // 6. Booking not found
        // ---------------------------------------------------------

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            // Arrange
            var request = CreateValidRequest();

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            bookingServiceMock
                .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
                .ReturnsAsync((Bookings?)null);

            // Act
            var response = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);

            bookingServiceMock.Verify(
                x => x.GetBookingById(request.BookingId, CancellationToken.None),
                Times.Once);

            mediaServiceMock.Verify(
                x => x.AddAsync(
                    It.IsAny<Media>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ---------------------------------------------------------
        // 7. Booking doesn't belong to current customer
        // ---------------------------------------------------------

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenBookingDoesNotBelongToUser()
        {
            // Arrange
            var request = CreateValidRequest();

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var booking = new Bookings
            {
                Id = request.BookingId,
                CustomerId = "customer-2"
            };

            bookingServiceMock
                .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            // Act
            var response = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);

            mediaServiceMock.Verify(
                x => x.AddAsync(
                    It.IsAny<Media>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ---------------------------------------------------------
        // 8. Mapper creates Media and UploadedByUserId is assigned
        // ---------------------------------------------------------

        [Fact]
        public async Task Handle_ShouldAddMediaSuccessfully_WhenRequestIsValid()
        {
            // Arrange
            var request = CreateValidRequest();

            const string customerId = "customer-1";

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(customerId);

            var booking = new Bookings
            {
                Id = request.BookingId,
                CustomerId = customerId
            };

            var media = new Media();

            bookingServiceMock
                .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            mapperMock
                .Setup(x => x.Map<Media>(request))
                .Returns(media);

            mediaServiceMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Media>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var response = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);

            Assert.Equal(
                customerId,
                media.UploadedByUserId);

            mapperMock.Verify(
                x => x.Map<Media>(request),
                Times.Once);

            mediaServiceMock.Verify(
                x => x.AddAsync(
                    media,
                    request.File,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // ---------------------------------------------------------
        // 9. MediaService throws exception
        // ---------------------------------------------------------

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenMediaUploadFails()
        {
            // Arrange
            var request = CreateValidRequest();

            const string customerId = "customer-1";

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(customerId);

            var booking = new Bookings
            {
                Id = request.BookingId,
                CustomerId = customerId
            };

            var media = new Media();

            bookingServiceMock
                .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            mapperMock
                .Setup(x => x.Map<Media>(request))
                .Returns(media);

            mediaServiceMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Media>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Upload failed"));

            // Act
            var response = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);

            mediaServiceMock.Verify(
                x => x.AddAsync(
                    media,
                    request.File,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // ---------------------------------------------------------
        // 10. Correct BookingId is sent to service
        // ---------------------------------------------------------

        [Fact]
        public async Task Handle_ShouldCallGetBookingById_WithCorrectBookingId()
        {
            // Arrange
            var request = CreateValidRequest();

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var booking = new Bookings
            {
                Id = request.BookingId,
                CustomerId = "customer-1"
            };

            var media = new Media();

            bookingServiceMock
                .Setup(x => x.GetBookingById(request.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            mapperMock
                .Setup(x => x.Map<Media>(request))
                .Returns(media);

            mediaServiceMock
                .Setup(x => x.AddAsync(
                    It.IsAny<Media>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            bookingServiceMock.Verify(
                x => x.GetBookingById(request.BookingId, CancellationToken.None),
                Times.Once);
        }
    }
}