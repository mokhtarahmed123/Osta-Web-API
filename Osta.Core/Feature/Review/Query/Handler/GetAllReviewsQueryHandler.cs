using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Review.Query.Model;
using Osta.Core.Feature.Review.Query.Result;
using Osta.Service.Abstract.ReviewAbstract;

namespace Osta.Core.Feature.Review.Query.Handler
{
    public class GetAllReviewsQueryHandler : ResponseHandler,
        IRequestHandler<
            GetAllReviewsQuery,
            Response<List<GetAllReviewsResult>>>
    {
        private readonly IReviewService reviewService;
        private readonly IMapper mapper;


        public GetAllReviewsQueryHandler(IReviewService reviewService, IMapper mapper)
        {
            this.reviewService = reviewService;
            this.mapper = mapper;

        }

        public async Task<
    Response<List<GetAllReviewsResult>>>
    Handle(
        GetAllReviewsQuery request,
        CancellationToken cancellationToken)
        {
            var reviews =
                await reviewService.GetAll(
                    cancellationToken);

            var result =
                mapper.Map<List<GetAllReviewsResult>>(
                    reviews);

            return Success(result);
        }
    }
}
