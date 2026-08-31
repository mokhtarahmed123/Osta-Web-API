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
    public class GetReviewByIdQueryTesting
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ICurrentUserService> currentUserServiceMock;
        private readonly Mock<IReviewService> reviewServiceMock;

        private readonly GetReviewByIdQueryHandler handler;

        public GetReviewByIdQueryTesting()
        {
            mapperMock = new Mock<IMapper>();
            currentUserServiceMock = new Mock<ICurrentUserService>();
            reviewServiceMock = new Mock<IReviewService>();

            handler = new GetReviewByIdQueryHandler(
                reviewServiceMock.Object,
                mapperMock.Object,
                currentUserServiceMock.Object
            );
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenIdIsLessThanOrEqualToZero()
        {
            // Arrange
            var request = new GetReviewByIdQuery(-1)
;

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenReviewDoesNotExist()
        {
            // Arrange
            var request = new GetReviewByIdQuery(1)
          ;
            reviewServiceMock
                .Setup(x => x.GetReview(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Review?)null);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

            reviewServiceMock.Verify(
                x => x.GetReview(
                    request.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mapperMock.Verify(
                x => x.Map<GetReviewByIdResult>(
                    It.IsAny<Review>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedReview_WhenReviewExists()
        {
            // Arrange
            var request = new GetReviewByIdQuery(1);

            var review = new Review();

            var mappedReview = new GetReviewByIdResult();

            reviewServiceMock
                .Setup(x => x.GetReview(
                    request.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            mapperMock
                .Setup(x => x.Map<GetReviewByIdResult>(review))
                .Returns(mappedReview);

            // Act
            var result = await handler.Handle(
                request,
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Same(mappedReview, result.Data);

            reviewServiceMock.Verify(
                x => x.GetReview(
                    request.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mapperMock.Verify(
                x => x.Map<GetReviewByIdResult>(review),
                Times.Once);
        }

    }
}
