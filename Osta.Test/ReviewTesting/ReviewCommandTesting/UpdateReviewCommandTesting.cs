using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Booking.Interface;
using Osta.Core.Feature.Review.Command.Handler;
using Osta.Core.Feature.Review.Command.Model;
using Osta.Data.Entities;
using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Service.Abstract.ReviewAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.ReviewTesting.ReviewCommandTesting
{
    public class UpdateReviewCommandTesting
    {

        private readonly Mock<ICurrentUserService> currentUserServiceMock;
        private readonly Mock<IReviewService> reviewServiceMock;
        private readonly Mock<IBookingService> bookingServiceMock;
        private readonly Mock<UserManager<User>> userManagerMock;

        private readonly UpdateReviewCommandHandler handler;

        public UpdateReviewCommandTesting()
        {

            currentUserServiceMock = new Mock<ICurrentUserService>();
            reviewServiceMock = new Mock<IReviewService>();
            bookingServiceMock = new Mock<IBookingService>();


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

            handler = new UpdateReviewCommandHandler(

                currentUserServiceMock.Object,
                reviewServiceMock.Object,
                bookingServiceMock.Object,
                userManagerMock.Object
                );
        }


        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Arrange
            UpdateReviewCommand request = null!;

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

            var request = new UpdateReviewCommand(1)
            {
                Id = 1,
                Rating = 5,
                Comment = "Excellent"
            };

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

            var request = new UpdateReviewCommand(1)
            {
                Id = 1,
                Rating = 5,
                Comment = "Excellent"
            };

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

            var request = new UpdateReviewCommand(1)
            {
                Id = 1,
                Rating = 5,
                Comment = "Excellent"
            };

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

            bookingServiceMock.Verify(
                x => x.GetBookingById(
                    It.IsAny<int>(), CancellationToken.None),
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

            var request = new UpdateReviewCommand(10)
            {
                Id = 10,
                Rating = 4,
                Comment = "Good"
            };

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
                x => x.Update(
                    It.IsAny<int>(),
                    It.IsAny<Review>(),
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
                BookingId = 20,
                Rating = 3,
                Comment = "Old comment"
            };

            reviewServiceMock
                .Setup(x => x.GetReview(
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            bookingServiceMock
                .Setup(x => x.GetBookingById(20, CancellationToken.None))
                .ReturnsAsync((Bookings?)null);

            var request = new UpdateReviewCommand(10)
            {
                Id = 10,
                Rating = 5,
                Comment = "Excellent"
            };

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
                x => x.Update(
                    It.IsAny<int>(),
                    It.IsAny<Review>(),
                    It.IsAny<CancellationToken>()),
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
                BookingId = 20,
                Rating = 3,
                Comment = "Old comment"
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

            var request = new UpdateReviewCommand(10)
            {
                Id = 10,
                Rating = 5,
                Comment = "Excellent"
            };

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            reviewServiceMock.Verify(
                x => x.Update(
                    It.IsAny<int>(),
                    It.IsAny<Review>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }


        [Fact]
        public async Task Handle_ShouldUpdateReviewSuccessfully()
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
                BookingId = 20,
                Rating = 3,
                Comment = "Old comment"
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

            var request = new UpdateReviewCommand(10)
            {
                Id = 10,
                Rating = 5,
                Comment = "Excellent service"
            };

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(5, review.Rating);
            Assert.Equal("Excellent service", review.Comment);

            reviewServiceMock.Verify(
                x => x.Update(
                    10,
                    review,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldUpdateOnlyRatingAndComment()
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
                BookingId = 20,
                Rating = 2,
                Comment = "Bad"
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

            var request = new UpdateReviewCommand(10)
            {
                Id = 10,
                Rating = 4,
                Comment = "Much better"
            };

            // Act
            await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(4, review.Rating);
            Assert.Equal("Much better", review.Comment);

            Assert.Equal(10, review.Id);
            Assert.Equal(20, review.BookingId);
        }
    }
}