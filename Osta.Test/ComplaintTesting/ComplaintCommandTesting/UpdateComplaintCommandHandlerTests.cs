using Moq;
using Osta.Core.Feature.Complaint.Command.Handler;
using Osta.Core.Feature.Complaint.Command.Model;
using Osta.Data.Entities.Booking;
using Osta.Data.Enum;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.ComplaintTesting.ComplaintCommandTesting
{
    public class UpdateComplaintCommandHandlerTests
    {

        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IComplaintService> _complaintServiceMock;
        private readonly UpdateComplaintCommandHandler _handler;

        public UpdateComplaintCommandHandlerTests()
        {

            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _complaintServiceMock = new Mock<IComplaintService>();


            _handler = new UpdateComplaintCommandHandler(

                _currentUserServiceMock.Object,
                _complaintServiceMock.Object


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
        public async Task Handle_ShouldReturnBadRequest_WhenComplaintIdIsInvalid()
        {
            // Arrange
            var request = new UpdateComplaintCommand(0)
            {
                Id = 0,
                Description = "Updated description"
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

            _complaintServiceMock.Verify(
                x => x.GetById(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
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

            var request = CreateRequest();

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
                x => x.Update(
                    It.IsAny<int>(),
                    It.IsAny<Data.Entities.Administration.Complaint>(),
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

            var request = CreateRequest();

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
                "You cannot update this complaint.",
                result.Message);

            _complaintServiceMock.Verify(
                x => x.Update(
                    It.IsAny<int>(),
                    It.IsAny<Data.Entities.Administration.Complaint>(),
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

            var request = CreateRequest();

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
                "You cannot update a resolved complaint.",
                result.Message);

            _complaintServiceMock.Verify(
                x => x.Update(
                    It.IsAny<int>(),
                    It.IsAny<Data.Entities.Administration.Complaint>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldUpdateComplaintSuccessfully_WhenRequestIsValid()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var request = CreateRequest();

            var complaint = CreateComplaint(
                customerId: "customer-1",
                status: ComplaintStatus.Open);

            _complaintServiceMock
                .Setup(x => x.GetById(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaint);

            _complaintServiceMock
                .Setup(x => x.Update(
                    request.Id,
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
            //    "Complaint updated successfully.",
            //    result.Message);

            Assert.Equal(
                request.Description,
                complaint.Description);

            _complaintServiceMock.Verify(
                x => x.Update(
                    request.Id,
                    complaint,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUpdateOnlyDescription_WhenRequestIsValid()
        {
            // Arrange
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns("customer-1");

            var request = CreateRequest();

            var complaint = CreateComplaint(
                customerId: "customer-1",
                status: ComplaintStatus.Open);

            var originalStatus = complaint.Status;
            var originalBookingId = complaint.BookingId;

            _complaintServiceMock
                .Setup(x => x.GetById(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaint);

            _complaintServiceMock
                .Setup(x => x.Update(
                    request.Id,
                    It.IsAny<Data.Entities.Administration.Complaint>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(request.Description, complaint.Description);

            Assert.Equal(originalStatus, complaint.Status);
            Assert.Equal(originalBookingId, complaint.BookingId);
        }

        private static UpdateComplaintCommand CreateRequest()
        {
            return new UpdateComplaintCommand(1)
            {
                Id = 1,
                Description = "Updated complaint description"
            };
        }

        private static Data.Entities.Administration.Complaint CreateComplaint(
            string customerId,
            ComplaintStatus status)
        {
            return new Data.Entities.Administration.Complaint
            {
                Id = 1,
                Description = "Old complaint description",
                Status = status,
                BookingId = 1,
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

