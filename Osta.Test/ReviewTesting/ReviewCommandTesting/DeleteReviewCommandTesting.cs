using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Booking.Interface;
using Osta.Core.Feature.Review.Command.Handler;
using Osta.Core.Feature.Review.Command.Model;
using Osta.Data.Entities;
using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Service.Abstract.ReviewAbstract;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.ReviewTesting.ReviewCommandTesting
{
    public class DeleteReviewCommandTesting
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ICurrentUserService> currentUserServiceMock;
        private readonly Mock<IReviewService> reviewServiceMock;
        private readonly Mock<IBookingService> bookingServiceMock;
        private readonly Mock<ITechnicianService> technicianServiceMock;
        private readonly Mock<UserManager<User>> userManagerMock;

        private readonly DeleteReviewCommandHandler handler;

        public DeleteReviewCommandTesting()
        {
            mapperMock = new Mock<IMapper>();
            currentUserServiceMock = new Mock<ICurrentUserService>();
            reviewServiceMock = new Mock<IReviewService>();
            bookingServiceMock = new Mock<IBookingService>();
            technicianServiceMock = new Mock<ITechnicianService>();

            var userStoreMock = new Mock<IUserStore<User>>();

            userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            handler = new DeleteReviewCommandHandler(
                mapperMock.Object,
                currentUserServiceMock.Object,
                reviewServiceMock.Object,
                bookingServiceMock.Object,
                userManagerMock.Object,
                technicianServiceMock.Object);
        }


        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Arrange
            DeleteReviewCommand request = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => handler.Handle(
                    request,
                    CancellationToken.None));
        }


        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIdIsEmpty()
        {
            // Arrange
            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var request = new DeleteReviewCommand(1);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(
                    request,
                    CancellationToken.None));
        }


        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            userManagerMock
                .Setup(x => x.FindByIdAsync("customer-1"))
                .ReturnsAsync((User?)null);

            var request = new DeleteReviewCommand(1);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            userManagerMock.Verify(
                x => x.FindByIdAsync("customer-1"),
                Times.Once);

            reviewServiceMock.Verify(
                x => x.GetReview(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }


        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotCustomer()
        {
            // Arrange
            var user = new User
            {
                Id = "user-1"
            };

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("user-1");

            userManagerMock
                .Setup(x => x.FindByIdAsync("user-1"))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>
                {
                    "Technician"
                });

            var request = new DeleteReviewCommand(1);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            reviewServiceMock.Verify(
                x => x.GetReview(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }


        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenReviewDoesNotExist()
        {
            // Arrange
            var user = new User
            {
                Id = "customer-1"
            };

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            userManagerMock
                .Setup(x => x.FindByIdAsync("customer-1"))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>
                {
                    "User"
                });

            reviewServiceMock
                .Setup(x => x.GetReview(
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Review?)null);

            var request = new DeleteReviewCommand(10);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            reviewServiceMock.Verify(
                x => x.GetReview(
                    10,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            bookingServiceMock.Verify(
                x => x.GetBookingById(
                    It.IsAny<int>(), CancellationToken.None),
                Times.Never);

            reviewServiceMock.Verify(
                x => x.Delete(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }


        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            // Arrange
            var user = new User
            {
                Id = "customer-1"
            };

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            userManagerMock
                .Setup(x => x.FindByIdAsync("customer-1"))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>
                {
                    "User"
                });

            var review = new Review
            {
                Id = 10,
                BookingId = 20
            };

            reviewServiceMock
                .Setup(x => x.GetReview(
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            bookingServiceMock
                .Setup(x => x.GetBookingById(20, CancellationToken.None))
                .ReturnsAsync((Bookings?)null);

            var request = new DeleteReviewCommand(10);


            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            bookingServiceMock.Verify(
                x => x.GetBookingById(20, CancellationToken.None),
                Times.Once);

            reviewServiceMock.Verify(
                x => x.Delete(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            technicianServiceMock.Verify(
                x => x.UpdateReviewCount(
                    It.IsAny<string>(),
                    It.IsAny<int>(), CancellationToken.None),
                Times.Never);
        }


        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenBookingDoesNotBelongToUser()
        {
            // Arrange
            var user = new User
            {
                Id = "customer-1"
            };

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            userManagerMock
                .Setup(x => x.FindByIdAsync("customer-1"))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>
                {
                    "User"
                });

            var review = new Review
            {
                Id = 10,
                BookingId = 20
            };

            reviewServiceMock
                .Setup(x => x.GetReview(
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var booking = new Bookings
            {
                CustomerId = "customer-2",
                TechnicianId = "technician-1"
            };

            bookingServiceMock
                .Setup(x => x.GetBookingById(20, CancellationToken.None))
                .ReturnsAsync(booking);

            var request = new DeleteReviewCommand(10);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            reviewServiceMock.Verify(
                x => x.Delete(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            technicianServiceMock.Verify(
                x => x.UpdateReviewCount(
                    It.IsAny<string>(),
                    It.IsAny<int>(), CancellationToken.None),
                Times.Never);
        }


        [Fact]
        public async Task Handle_ShouldDeleteReviewSuccessfully()
        {
            // Arrange
            var user = new User
            {
                Id = "customer-1"
            };

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            userManagerMock
                .Setup(x => x.FindByIdAsync("customer-1"))
                .ReturnsAsync(user);

            userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>
                {
                    "User"
                });

            var review = new Review
            {
                Id = 10,
                BookingId = 20
            };

            reviewServiceMock
                .Setup(x => x.GetReview(
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var booking = new Bookings
            {
                CustomerId = "customer-1",
                TechnicianId = "technician-1"
            };

            bookingServiceMock
                .Setup(x => x.GetBookingById(20, CancellationToken.None))
                .ReturnsAsync(booking);

            var request = new DeleteReviewCommand(10);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            reviewServiceMock.Verify(
                x => x.GetReview(
                    10,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            bookingServiceMock.Verify(
                x => x.GetBookingById(20, CancellationToken.None),
                Times.Once);

            reviewServiceMock.Verify(
                x => x.Delete(
                    10,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            technicianServiceMock.Verify(
                x => x.UpdateReviewCount(
                    "technician-1",
                    -1, CancellationToken.None),
                Times.Once);
        }
    }
}