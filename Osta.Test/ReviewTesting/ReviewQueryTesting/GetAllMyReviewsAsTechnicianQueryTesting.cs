using AutoMapper;
using Moq;
using Osta.Core.Feature.Review.Query.Handler;
using Osta.Core.Feature.Review.Query.Model;
using Osta.Core.Feature.Review.Query.Result;
using Osta.Data.Entities;
using Osta.Service.Abstract.ReviewAbstract;
using Osta.SharedKernel.Identity;
using System.Net;

namespace Osta.Test.ReviewTesting.ReviewQueryTesting
{
    public class GetAllMyReviewsAsTechnicianQueryTesting
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ICurrentUserService> currentUserServiceMock;
        private readonly Mock<IReviewService> reviewServiceMock;

        private readonly GetAllMyReviewsAsTechnicianQueryHandler handler;

        public GetAllMyReviewsAsTechnicianQueryTesting()
        {
            mapperMock = new Mock<IMapper>();
            currentUserServiceMock = new Mock<ICurrentUserService>();
            reviewServiceMock = new Mock<IReviewService>();

            handler = new GetAllMyReviewsAsTechnicianQueryHandler(
                reviewServiceMock.Object,
                mapperMock.Object,
                currentUserServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIdIsEmpty()
        {
            // Arrange
            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(string.Empty);

            var request = new GetAllMyReviewsAsTechnicianQuery();

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

            var request = new GetAllMyReviewsAsTechnicianQuery();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedReviews_WhenTechnicianRequestsAllReviews()
        {
            // Arrange
            const string technicianId = "TechId";

            currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(technicianId);

            var request = new GetAllMyReviewsAsTechnicianQuery();

            var reviews = new List<Review>
            {
                new Review(),
                new Review()
            };

            var mappedReviews = new List<GetAllMyReviewsAsTechnicianResult>
            {
                new GetAllMyReviewsAsTechnicianResult(),
                new GetAllMyReviewsAsTechnicianResult()
            };

            reviewServiceMock
                .Setup(x => x.GetAllMyReviewAsTechnician(
                    technicianId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(reviews);

            mapperMock
                .Setup(x => x.Map<List<GetAllMyReviewsAsTechnicianResult>>(
                    reviews))
                .Returns(mappedReviews);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Same(mappedReviews, result.Data);

            reviewServiceMock.Verify(
                x => x.GetAllMyReviewAsTechnician(
                    technicianId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mapperMock.Verify(
                x => x.Map<List<GetAllMyReviewsAsTechnicianResult>>(reviews),
                Times.Once);
        }
    }
}