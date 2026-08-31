using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Review.Query.Model;
using Osta.Core.Feature.Review.Query.Result;
using Osta.Service.Abstract.ReviewAbstract;

namespace Osta.Core.Feature.Review.Query.Handler
{
    public class GetReviewByIdQueryHandler : ResponseHandler,
        IRequestHandler<
            GetReviewByIdQuery,
            Response<GetReviewByIdResult>>
    {
        private readonly IReviewService reviewService;
        private readonly IMapper mapper;


        public GetReviewByIdQueryHandler(IReviewService reviewService, IMapper mapper)
        {
            this.reviewService = reviewService;
            this.mapper = mapper;

        }


        public async Task<
            Response<GetReviewByIdResult>>
            Handle(
                GetReviewByIdQuery request,
                CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                return BadRequest<GetReviewByIdResult>(
                    "Review Id must be greater than 0.");

            var review =
                await reviewService.GetReview(
                    request.Id,
                    cancellationToken);

            if (review is null)
                return NotFound<GetReviewByIdResult>(
                    "Review not found.");

            var result =
                mapper.Map<GetReviewByIdResult>(
                    review);

            return Success(result);
        }
    }
}
