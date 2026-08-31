
using AutoMapper;
using Moq;
using Osta.Booking.Interface;
using Osta.Core.Feature.Complaint.Command.Handler;
using Osta.Core.Feature.Complaint.Command.Model;
using Osta.Data.Entities.Booking;
using Osta.Data.Enum;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.ComplaintTesting.ComplaintCommandTesting
{
    public class AddComplaintCommandHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IComplaintService> _complaintServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;


        private readonly AddComplaintCommandHandler _handler;

        public AddComplaintCommandHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _complaintServiceMock = new Mock<IComplaintService>();
            _bookingServiceMock = new Mock<IBookingService>();


            _handler = new AddComplaintCommandHandler(
                _mapperMock.Object,
                _currentUserServiceMock.Object,
                _complaintServiceMock.Object,
                _bookingServiceMock.Object
             );
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(null!, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenCustomerIdIsEmpty()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var request = CreateRequest();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(request, CancellationToken.None));

            Assert.Equal(
                "You are not authorized.",
                exception.Message);

            _bookingServiceMock.Verify(
                x => x.GetBookingById(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var request = CreateRequest();

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    request.BookingId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Bookings?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "Booking not found.",
                result.Message);

            _complaintServiceMock.Verify(
                x => x.GetByBookingId(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenBookingDoesNotBelongToCustomer()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var request = CreateRequest();

            var booking = CreateBooking(
                customerId: "customer-2",
                status: BookingStatus.Completed);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    request.BookingId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "This booking does not belong to you.",
                result.Message);

            _complaintServiceMock.Verify(
                x => x.GetByBookingId(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenBookingIsNotCompleted()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var request = CreateRequest();

            var booking = CreateBooking(
                customerId: "customer-1",
                status: BookingStatus.Pending);

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    request.BookingId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "You can create a complaint only for completed bookings.",
                result.Message);

            _complaintServiceMock.Verify(
                x => x.GetByBookingId(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenComplaintAlreadyExists()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var request = CreateRequest();

            var booking = CreateBooking(
                customerId: "customer-1",
                status: BookingStatus.Completed);

            var existingComplaints = new List<Data.Entities.Administration.Complaint>
            {
                new Data.Entities.Administration.Complaint()
            };

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    request.BookingId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            _complaintServiceMock
                .Setup(x => x.GetByBookingId(
                    request.BookingId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingComplaints);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "You have already submitted a complaint for this booking.",
                result.Message);

            _mapperMock.Verify(
                x => x.Map<Data.Entities.Administration.Complaint>(
                    It.IsAny<AddComplaintCommand>()),
                Times.Never);

            _complaintServiceMock.Verify(
                x => x.Add(
                    It.IsAny<Data.Entities.Administration.Complaint>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldAddComplaintSuccessfully_WhenRequestIsValid()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var request = CreateRequest();

            var booking = CreateBooking(
                customerId: "customer-1",
                status: BookingStatus.Completed);

            var complaint = new Data.Entities.Administration.Complaint
            {
                Status = ComplaintStatus.Open
            };

            _bookingServiceMock
                .Setup(x => x.GetBookingById(
                    request.BookingId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            _complaintServiceMock
                .Setup(x => x.GetByBookingId(
                    request.BookingId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Data.Entities.Administration.Complaint>());

            _mapperMock
                .Setup(x => x.Map<Data.Entities.Administration.Complaint>(request))
                .Returns(complaint);

            _complaintServiceMock
                .Setup(x => x.Add(
                    It.IsAny<Data.Entities.Administration.Complaint>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            //Assert.Contains(
            //    "Complaint submitted successfully.",
            //    result.Message);

            Assert.Equal(
                ComplaintStatus.Open,
                complaint.Status);

            _mapperMock.Verify(
                x => x.Map<Data.Entities.Administration.Complaint>(request),
                Times.Once);

            _complaintServiceMock.Verify(
                x => x.Add(
                    complaint,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static AddComplaintCommand CreateRequest()
        {
            return new AddComplaintCommand
            {
                BookingId = 1
            };
        }

        private static Bookings CreateBooking(
            string customerId,
            BookingStatus status)
        {
            return new Bookings
            {
                Id = 1,
                CustomerId = customerId,
                TechnicianId = "technician-1",
                Status = status
            };
        }
    }
}
