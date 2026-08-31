
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Booking.Interface;
using Osta.Core.Feature.MediaBooking.Command.Handler;
using Osta.Core.Feature.MediaBooking.Command.Model;
using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Test.MediaBookingTesting.MediaBookingCommandTesting
{
    public class DeleteMediaBookingCommandHandlerTests
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly Mock<IMediaService> mediaServiceMock;
        private readonly Mock<IBookingService> bookingServiceMock;
        private readonly Mock<ICurrentUserService> currentUserServiceMock;
        private readonly Mock<UserManager<User>> userManagerMock;

        private readonly DeleteMediaBookingCommandHandler handler;

        public DeleteMediaBookingCommandHandlerTests()
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

            handler = new DeleteMediaBookingCommandHandler(
                mapperMock.Object,
                loggerMock.Object,
                mediaServiceMock.Object,
                bookingServiceMock.Object,
                currentUserServiceMock.Object,
                userManagerMock.Object);
        }

        // =========================================================
        // 1. Request is null
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Arrange
            DeleteMediaBookingCommand request = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => handler.Handle(
                    request,
                    CancellationToken.None));
        }

        // =========================================================
        // 2. Invalid Media Id
        // =========================================================

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenMediaIdIsLessThanOrEqualToZero()
        {
            // Arrange
            var request = new DeleteMediaBookingCommand(0);

            // Act
            var response = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);

            currentUserServiceMock.VerifyGet(
                x => x.UserId,
                Times.Never);

            mediaServiceMock.Verify(
                x => x.GetByIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // =========================================================
        // 3. User is not authenticated
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIdIsNull()
        {
            // Arrange
            var request = new DeleteMediaBookingCommand(1);

            currentUserServiceMock
                .SetupGet(x => x.UserId)
                .Returns((string?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(
                    request,
                    CancellationToken.None));

            mediaServiceMock.Verify(
                x => x.GetByIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // =========================================================
        // 4. UserId is empty
        // =========================================================

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIdIsEmpty()
        {
            // Arrange
            var request = new DeleteMediaBookingCommand(1);

            currentUserServiceMock
                .SetupGet(x => x.UserId)
                .Returns(string.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(
                    request,
                    CancellationToken.None));
        }

        // =========================================================
        // 5. Media not found
        // =========================================================

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenMediaDoesNotExist()
        {
            // Arrange
            var request = new DeleteMediaBookingCommand(1);

            currentUserServiceMock
                .SetupGet(x => x.UserId)
                .Returns("customer-1");

            mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Media?)null);

            // Act
            var response = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);

            bookingServiceMock.Verify(
                x => x.GetBookingById(
                    It.IsAny<int>(), CancellationToken.None),
                Times.Never);

            mediaServiceMock.Verify(
                x => x.DeleteAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // =========================================================
        // 6. Booking not found
        // =========================================================

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            // Arrange
            var request = new DeleteMediaBookingCommand(1);

            const string customerId = "customer-1";

            currentUserServiceMock
                .SetupGet(x => x.UserId)
                .Returns(customerId);

            var media = new Media
            {
                Id = request.Id,
                BookingId = 10,
                UploadedByUserId = customerId
            };

            mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            bookingServiceMock
                .Setup(x => x.GetBookingById(media.BookingId, CancellationToken.None))
                .ReturnsAsync((Bookings?)null);

            // Act
            var response = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);

            mediaServiceMock.Verify(
                x => x.DeleteAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // =========================================================
        // 7. Booking belongs to another customer
        // =========================================================

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenBookingDoesNotBelongToUser()
        {
            // Arrange
            var request = new DeleteMediaBookingCommand(1);

            const string customerId = "customer-1";

            currentUserServiceMock
                .SetupGet(x => x.UserId)
                .Returns(customerId);

            var media = new Media
            {
                Id = request.Id,
                BookingId = 10,
                UploadedByUserId = customerId
            };

            var booking = new Bookings
            {
                Id = media.BookingId,
                CustomerId = "customer-2"
            };

            mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            bookingServiceMock
                .Setup(x => x.GetBookingById(media.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            // Act
            var response = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);

            mediaServiceMock.Verify(
                x => x.DeleteAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // =========================================================
        // 8. Media was uploaded by another user
        // =========================================================

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenMediaWasUploadedByAnotherUser()
        {
            // Arrange
            var request = new DeleteMediaBookingCommand(1);

            const string customerId = "customer-1";

            currentUserServiceMock
                .SetupGet(x => x.UserId)
                .Returns(customerId);

            var media = new Media
            {
                Id = request.Id,
                BookingId = 10,
                UploadedByUserId = "customer-2"
            };

            var booking = new Bookings
            {
                Id = media.BookingId,
                CustomerId = customerId
            };

            mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            bookingServiceMock
                .Setup(x => x.GetBookingById(media.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            // Act
            var response = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);

            mediaServiceMock.Verify(
                x => x.DeleteAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // =========================================================
        // 9. Successful deletion
        // =========================================================

        [Fact]
        public async Task Handle_ShouldDeleteMediaSuccessfully_WhenUserOwnsBookingAndMedia()
        {
            // Arrange
            var request = new DeleteMediaBookingCommand(1);

            const string customerId = "customer-1";

            currentUserServiceMock
                .SetupGet(x => x.UserId)
                .Returns(customerId);

            var media = new Media
            {
                Id = request.Id,
                BookingId = 10,
                UploadedByUserId = customerId
            };

            var booking = new Bookings
            {
                Id = media.BookingId,
                CustomerId = customerId
            };

            mediaServiceMock
                .Setup(x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(media);

            bookingServiceMock
                .Setup(x => x.GetBookingById(media.BookingId, CancellationToken.None))
                .ReturnsAsync(booking);

            mediaServiceMock
                .Setup(x => x.DeleteAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var response = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);

            mediaServiceMock.Verify(
                x => x.GetByIdAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            bookingServiceMock.Verify(
                x => x.GetBookingById(
                    media.BookingId, CancellationToken.None),
                Times.Once);

            mediaServiceMock.Verify(
                x => x.DeleteAsync(
                    request.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
