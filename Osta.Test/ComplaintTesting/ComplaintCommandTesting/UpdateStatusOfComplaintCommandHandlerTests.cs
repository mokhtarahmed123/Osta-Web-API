using Moq;
using Osta.Core.Feature.Complaint.Command.Handler;
using Osta.Core.Feature.Complaint.Command.Model;
using Osta.Data.Enum;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Test.ComplaintTesting.ComplaintCommandTesting
{
    public class UpdateStatusOfComplaintCommandHandlerTests
    {

        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IComplaintService> _complaintServiceMock;

        private readonly Mock<ILoggerService> _loggerServiceMock;

        private readonly UpdateStatusOfComplaintCommandHandler _handler;

        public UpdateStatusOfComplaintCommandHandlerTests()
        {

            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _complaintServiceMock = new Mock<IComplaintService>();

            _loggerServiceMock = new Mock<ILoggerService>();

            _handler = new UpdateStatusOfComplaintCommandHandler(

                _currentUserServiceMock.Object,
                _complaintServiceMock.Object,

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
            var request = new UpdateStatusOfComplaintCommand(0, ComplaintStatus.UnderReview)
            {
                Id = 0,
                ComplaintStatus = ComplaintStatus.UnderReview
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
        public async Task Handle_ShouldReturnNotFound_WhenComplaintDoesNotExist()
        {
            // Arrange
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
                x => x.UpdateStatus(
                    It.IsAny<int>(),
                    It.IsAny<ComplaintStatus>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _loggerServiceMock.Verify(
                x => x.LogInformation(
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenComplaintIsAlreadyResolved()
        {
            // Arrange
            var request = CreateRequest();

            var complaint = CreateComplaint(
                ComplaintStatus.Resolved);

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
                "Resolved complaint cannot be updated.",
                result.Message);

            _complaintServiceMock.Verify(
                x => x.UpdateStatus(
                    It.IsAny<int>(),
                    It.IsAny<ComplaintStatus>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _loggerServiceMock.Verify(
                x => x.LogInformation(
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldUpdateStatusSuccessfully()
        {
            // Arrange
            var request = CreateRequest();

            var complaint = CreateComplaint(
                ComplaintStatus.Open);

            _complaintServiceMock
                .Setup(x => x.GetById(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaint);

            _complaintServiceMock
                .Setup(x => x.UpdateStatus(
                    request.Id,
                    request.ComplaintStatus,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            //Assert.Contains(
            //    $"Complaint status updated successfully to {request.ComplaintStatus}.",
            //    result.Message);

            _complaintServiceMock.Verify(
                x => x.UpdateStatus(
                    request.Id,
                    request.ComplaintStatus,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _loggerServiceMock.Verify(
                x => x.LogInformation(
                    It.Is<string>(message =>
                        message.Contains($"Complaint Id {request.Id}") &&
                        message.Contains(complaint.Status.ToString()) &&
                        message.Contains(request.ComplaintStatus.ToString()))),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUpdateStatusFromOpenToResolved()
        {
            // Arrange
            var request = new UpdateStatusOfComplaintCommand(1, ComplaintStatus.Resolved)
            {
                Id = 1,
                ComplaintStatus = ComplaintStatus.Resolved
            };

            var complaint = CreateComplaint(
                ComplaintStatus.Open);

            _complaintServiceMock
                .Setup(x => x.GetById(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaint);

            _complaintServiceMock
                .Setup(x => x.UpdateStatus(
                    request.Id,
                    request.ComplaintStatus,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            //Assert.Contains(
            //    "Resolved",
            //    result.Message);

            _complaintServiceMock.Verify(
                x => x.UpdateStatus(
                    request.Id,
                    ComplaintStatus.Resolved,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _loggerServiceMock.Verify(
                x => x.LogInformation(
                    It.Is<string>(message =>
                        message.Contains("Open") &&
                        message.Contains("Resolved"))),
                Times.Once);
        }

        private static UpdateStatusOfComplaintCommand CreateRequest()
        {
            return new UpdateStatusOfComplaintCommand(1, ComplaintStatus.UnderReview)
            {
                Id = 1,
                ComplaintStatus = ComplaintStatus.UnderReview
            };
        }

        private static Data.Entities.Administration.Complaint CreateComplaint(
            ComplaintStatus status)
        {
            return new Data.Entities.Administration.Complaint
            {
                Id = 1,
                Status = status,
                Description = "Test complaint",
                BookingId = 1
            };
        }
    }
}