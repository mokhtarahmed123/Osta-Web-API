
using AutoMapper;
using Moq;
using Osta.Core.Feature.Complaint.Query.Handler;
using Osta.Core.Feature.Complaint.Query.Model;
using Osta.Core.Feature.Complaint.Query.Result;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Test.ComplaintTesting.ComplaintQueryTesting
{
    public class GetAllComplaintQueryHandlerTests
    {
        private readonly Mock<IComplaintService> _complaintServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly GetAllComplaintQueryHandler _handler;

        public GetAllComplaintQueryHandlerTests()
        {
            _complaintServiceMock = new Mock<IComplaintService>();
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new GetAllComplaintQueryHandler(
                _complaintServiceMock.Object,
                _mapperMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnComplaintsSuccessfully()
        {
            // Arrange
            var complaints = new List<Data.Entities.Administration.Complaint>
            {
                new Data.Entities.Administration.Complaint
                {
                    Id = 1,
                    Description = "Complaint 1"
                },
                new Data.Entities.Administration.Complaint
                {
                    Id = 2,
                    Description = "Complaint 2"
                }
            };

            var mappedResult = new List<GetAllComplaintResult>
            {
                new GetAllComplaintResult(),
                new GetAllComplaintResult()
            };

            _complaintServiceMock
                .Setup(x => x.GetAllComplaints(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaints);

            _mapperMock
                .Setup(x => x.Map<List<GetAllComplaintResult>>(complaints))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(
                new GetAllComplaintQuery(),
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Equal(2, result.Data.Count);
            Assert.Same(mappedResult, result.Data);

            _complaintServiceMock.Verify(
                x => x.GetAllComplaints(
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetAllComplaintResult>>(complaints),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenThereAreNoComplaints()
        {
            // Arrange
            var complaints = new List<Data.Entities.Administration.Complaint>();

            var mappedResult = new List<GetAllComplaintResult>();

            _complaintServiceMock
                .Setup(x => x.GetAllComplaints(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaints);

            _mapperMock
                .Setup(x => x.Map<List<GetAllComplaintResult>>(complaints))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(
                new GetAllComplaintQuery(),
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            Assert.Empty(result.Data);

            _complaintServiceMock.Verify(
                x => x.GetAllComplaints(
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<GetAllComplaintResult>>(complaints),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldMapComplaintsToGetAllComplaintResult()
        {
            // Arrange
            var complaints = new List<Data.Entities.Administration.Complaint>
            {
                new Data.Entities.Administration.Complaint
                {
                    Id = 1,
                    Description = "Test complaint"
                }
            };

            var mappedResult = new List<GetAllComplaintResult>
            {
                new GetAllComplaintResult()
            };

            _complaintServiceMock
                .Setup(x => x.GetAllComplaints(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(complaints);

            _mapperMock
                .Setup(x => x.Map<List<GetAllComplaintResult>>(complaints))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(
                new GetAllComplaintQuery(),
                CancellationToken.None);

            // Assert
            Assert.Same(mappedResult, result.Data);

            _mapperMock.Verify(
                x => x.Map<List<GetAllComplaintResult>>(complaints),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToService()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var complaints = new List<Data.Entities.Administration.Complaint>();

            _complaintServiceMock
                .Setup(x => x.GetAllComplaints(cancellationToken))
                .ReturnsAsync(complaints);

            _mapperMock
                .Setup(x => x.Map<List<GetAllComplaintResult>>(complaints))
                .Returns(new List<GetAllComplaintResult>());

            // Act
            await _handler.Handle(
                new GetAllComplaintQuery(),
                cancellationToken);

            // Assert
            _complaintServiceMock.Verify(
                x => x.GetAllComplaints(cancellationToken),
                Times.Once);
        }
    }
}

