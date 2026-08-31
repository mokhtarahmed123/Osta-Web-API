
using AutoMapper;
using Moq;
using Osta.Booking.Interface;
using Osta.Core.Feature.Complaint.Command.Handler;
using Osta.Core.Feature.Complaint.Command.Model;
using Osta.Data.Entities.Booking;
using Osta.Data.Enum;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;
namespace Osta.Test.ComplaintTesting.ComplaintCommandTesting
{
    public class DeleteComplaintCommandHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IComplaintService> _complaintServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<ILoggerService> _loggerServiceMock;

        private readonly DeleteComplaintCommandHandler _handler;

        public DeleteComplaintCommandHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _complaintServiceMock = new Mock<IComplaintService>();
            _bookingServiceMock = new Mock<IBookingService>();
            _loggerServiceMock = new Mock<ILoggerService>();

            _handler = new DeleteComplaintCommandHandler(
                _mapperMock.Object,
                _currentUserServiceMock.Object,
                _complaintServiceMock.Object,
                _bookingServiceMock.Object,
                _loggerServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(null!, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenComplaintIdIsInvalid()
        {
            // Arrange
            var request = new DeleteComplaintCommand(0)
            {
                Id = 0
            };

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "Complaint ID must be greater than 0.",
                result.Message);

            _currentUserServiceMock.Verify(
                x => x.UserId,
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenCustomerIdIsEmpty()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var request = new DeleteComplaintCommand(1)
            {
                Id = 1
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(request, CancellationToken.None));

            Assert.Equal(
                "You are not authorized.",
                exception.Message);

            _complaintServiceMock.Verify(
                x => x.GetById(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenComplaintDoesNotExist()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var request = new DeleteComplaintCommand(1)
            {
                Id = 1
            };

            _complaintServiceMock
                .Setup(x => x.GetById(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Data.Entities.Administration.Complaint?)null);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "Complaint not found.",
                result.Message);

            _complaintServiceMock.Verify(
                x => x.Delete(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenComplaintDoesNotBelongToCustomer()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var request = new DeleteComplaintCommand(1)
            {
                Id = 1
            };

            var complaint = CreateComplaint(
                customerId: "customer-2",
                status: ComplaintStatus.Open);

            _complaintServiceMock
                .Setup(x => x.GetById(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaint);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "You cannot delete this complaint.",
                result.Message);

            _complaintServiceMock.Verify(
                x => x.Delete(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenComplaintIsResolved()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var request = new DeleteComplaintCommand(1)
            {
                Id = 1
            };

            var complaint = CreateComplaint(
                customerId: "customer-1",
                status: ComplaintStatus.Resolved);

            _complaintServiceMock
                .Setup(x => x.GetById(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaint);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Contains(
                "You cannot delete a resolved complaint.",
                result.Message);

            _complaintServiceMock.Verify(
                x => x.Delete(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldDeleteComplaintSuccessfully_WhenRequestIsValid()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var request = new DeleteComplaintCommand(1)
            {
                Id = 1
            };

            var complaint = CreateComplaint(
                customerId: "customer-1",
                status: ComplaintStatus.Open);

            _complaintServiceMock
                .Setup(x => x.GetById(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaint);

            _complaintServiceMock
                .Setup(x => x.Delete(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            //Assert.Contains(
            //    "Complaint deleted successfully.",
            //    result.Message);

            _complaintServiceMock.Verify(
                x => x.Delete(
                    request.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static Data.Entities.Administration.Complaint CreateComplaint(
            string customerId,
            ComplaintStatus status)
        {
            return new Data.Entities.Administration.Complaint
            {
                Id = 1,
                Status = status,
                Booking = new Bookings
                {
                    Id = 1,
                    CustomerId = customerId,
                    TechnicianId = "technician-1"
                }
            };
        }
    }
}

