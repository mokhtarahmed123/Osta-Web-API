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
    public class GetAllReviewsQueryTesting
    {
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ICurrentUserService> currentUserServiceMock;
        private readonly Mock<IReviewService> reviewServiceMock;

        private readonly GetAllReviewsQueryHandler handler;

        public GetAllReviewsQueryTesting()
        {
            mapperMock = new Mock<IMapper>();
            currentUserServiceMock = new Mock<ICurrentUserService>();
            reviewServiceMock = new Mock<IReviewService>();

            handler = new GetAllReviewsQueryHandler(
                reviewServiceMock.Object,
                mapperMock.Object,
                currentUserServiceMock.Object
            );
        }
        [Fact]
        public async Task Handle_ShouldReturnMappedReviews_WhenTechnicianRequestsAllReviews()
        {

            var request = new GetAllReviewsQuery();

            var reviews = new List<Review>
            {
                new Review(),
                new Review()
            };

            var mappedReviews = new List<GetAllReviewsResult>
            {
                new GetAllReviewsResult(),
                new GetAllReviewsResult()
            };

            reviewServiceMock
                .Setup(x => x.GetAll(

                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(reviews);

            mapperMock
                .Setup(x => x.Map<List<GetAllReviewsResult>>(
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
                x => x.GetAll(It.IsAny<CancellationToken>()),
                Times.Once);

            mapperMock.Verify(
                x => x.Map<List<GetAllReviewsResult>>(reviews),
                Times.Once);
        }


    }
}
