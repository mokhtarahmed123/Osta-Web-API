using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Osta.Booking.Interface;
using Osta.Core.Feature.Review.Command.Handler;
using Osta.Core.Feature.Review.Command.Model;
using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Data.Enum;
using Osta.Service.Abstract.ReviewAbstract;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.ReviewTesting.ReviewCommandTesting
{
    public class AddReviewCommandTesting
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ICurrentUserService> currentUserServiceMock;
        private readonly Mock<IReviewService> reviewServiceMock;
        private readonly Mock<IBookingService> bookingServiceMock;
        private readonly Mock<ITechnicianService> technicianServiceMock;

        private readonly Mock<UserManager<User>> userManagerMock;

        private readonly AddReviewCommandHandler handler;
        public AddReviewCommandTesting()
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

            handler = new AddReviewCommandHandler(
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
            AddReviewCommand request = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => handler.Handle(request, CancellationToken.None));
        }
        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIdIsEmpty()
        {
            // Arrange
            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var request = new AddReviewCommand();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(request, CancellationToken.None));
        }
        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIdIsNull()
        {
            // Arrange
            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns((string?)null);

            var request = new AddReviewCommand();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(request, CancellationToken.None));
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
            var request = new AddReviewCommand();
            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            userManagerMock.Verify(
                x => x.FindByIdAsync("customer-1"),
                Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotInUserRole()
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
            "Technician"
                });

            var request = new AddReviewCommand
            {
                Rating = 1,
                Comment = "Welcome",
                BookingId = 1
            };

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            userManagerMock.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            bookingServiceMock.Verify(
                x => x.GetBookingById(It.IsAny<int>(), It.IsAny<CancellationToken>()),
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

            bookingServiceMock
                .Setup(x => x.GetBookingById(10, It.IsAny<CancellationToken>()))
                   .ReturnsAsync((Bookings?)null);

            var request = new AddReviewCommand
            {
                BookingId = 10,
                Comment = "Comment",
                Rating = 2
            };

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            bookingServiceMock.Verify(
                x => x.GetBookingById(10, It.IsAny<CancellationToken>()),
                Times.Once);
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

            var booking = new Bookings
            {
                CustomerId = "customer-2",
                TechnicianId = "technician-1",
                Status = BookingStatus.Completed
            };

            bookingServiceMock
                .Setup(x => x.GetBookingById(10, CancellationToken.None))
                .ReturnsAsync(booking);

            var request = new AddReviewCommand
            {
                BookingId = 10,
                Rating = 4,
                Comment = "Comment"

            };

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            reviewServiceMock.Verify(
                x => x.GetByBookingId(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenBookingIsNotCompleted()
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

            var booking = new Bookings
            {
                CustomerId = "customer-1",
                TechnicianId = "technician-1",
                Status = BookingStatus.Pending
            };

            bookingServiceMock
                .Setup(x => x.GetBookingById(10, CancellationToken.None))
                .ReturnsAsync(booking);

            var request = new AddReviewCommand
            {
                BookingId = 10
            };

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            reviewServiceMock.Verify(
                x => x.GetByBookingId(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenBookingAlreadyHasReview()
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

            var booking = new Bookings
            {
                CustomerId = "customer-1",
                TechnicianId = "technician-1",
                Status = BookingStatus.Completed
            };

            bookingServiceMock
                .Setup(x => x.GetBookingById(10, CancellationToken.None))
                .ReturnsAsync(booking);

            var existingReview = new Data.Entities.Review();

            reviewServiceMock
                .Setup(x => x.GetByBookingId(
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingReview);

            var request = new AddReviewCommand
            {
                BookingId = 10
            };

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            reviewServiceMock.Verify(
                x => x.GetByBookingId(
                    10,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            reviewServiceMock.Verify(
                x => x.Add(
                    It.IsAny<Data.Entities.Review>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            technicianServiceMock.Verify(
                x => x.UpdateReviewCount(
                    It.IsAny<string>(),
                    It.IsAny<int>(), CancellationToken.None),
                Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldAddReviewSuccessfully_WhenAllConditionsAreValid()
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

            var booking = new Bookings
            {
                CustomerId = "customer-1",
                TechnicianId = "technician-1",
                Status = BookingStatus.Completed
            };

            bookingServiceMock
                .Setup(x => x.GetBookingById(10, CancellationToken.None))
                .ReturnsAsync(booking);

            reviewServiceMock
                .Setup(x => x.GetByBookingId(
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Data.Entities.Review?)null);

            var review = new Data.Entities.Review();

            mapperMock
                .Setup(x => x.Map<Data.Entities.Review>(
                    It.IsAny<AddReviewCommand>()))
                .Returns(review);

            var request = new AddReviewCommand
            {
                BookingId = 10
            };

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            mapperMock.Verify(
                x => x.Map<Data.Entities.Review>(
                    request),
                Times.Once);

            reviewServiceMock.Verify(
                x => x.Add(
                    review,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            technicianServiceMock.Verify(
                x => x.UpdateReviewCount(
                    "technician-1",
                    +1, CancellationToken.None),
                Times.Once);

            technicianServiceMock.Verify(
                x => x.RateTechnicianAsync(
                    "technician-1",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldAddMappedReview()
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

            var booking = new Bookings
            {
                CustomerId = "customer-1",
                TechnicianId = "technician-1",
                Status = BookingStatus.Completed
            };

            bookingServiceMock
                .Setup(x => x.GetBookingById(10, CancellationToken.None))
                .ReturnsAsync(booking);

            reviewServiceMock
                .Setup(x => x.GetByBookingId(
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Data.Entities.Review?)null);

            var mappedReview = new Data.Entities.Review();

            mapperMock
                .Setup(x => x.Map<Data.Entities.Review>(
                    It.IsAny<AddReviewCommand>()))
                .Returns(mappedReview);

            var request = new AddReviewCommand
            {
                BookingId = 10
            };

            // Act
            await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            reviewServiceMock.Verify(
                x => x.Add(
                    mappedReview,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUpdateCorrectTechnicianReviewCount()
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

            var booking = new Bookings
            {
                CustomerId = "customer-1",
                TechnicianId = "tech-999",
                Status = BookingStatus.Completed
            };

            bookingServiceMock
                .Setup(x => x.GetBookingById(10, CancellationToken.None))
                .ReturnsAsync(booking);

            reviewServiceMock
                .Setup(x => x.GetByBookingId(
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Data.Entities.Review?)null);

            mapperMock
                .Setup(x => x.Map<Data.Entities.Review>(
                    It.IsAny<AddReviewCommand>()))
                .Returns(new Data.Entities.Review());

            // Act
            await handler.Handle(
                new AddReviewCommand
                {
                    BookingId = 10
                },
                CancellationToken.None);

            // Assert
            technicianServiceMock.Verify(
                x => x.UpdateReviewCount(
                    "tech-999",
                    +1, CancellationToken.None),
                Times.Once);
        }
    }
}
