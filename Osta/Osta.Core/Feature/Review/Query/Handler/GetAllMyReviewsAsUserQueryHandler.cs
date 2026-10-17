using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Review.Query.Model;
using Osta.Core.Feature.Review.Query.Result;
using Osta.Service.Abstract.ReviewAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Review.Query.Handler
{
    public class GetAllMyReviewsAsUserQueryHandler : ResponseHandler, IRequestHandler<
            GetAllMyReviewsAsUserQuery,
            Response<List<GetAllMyReviewsAsUserResult>>>
    {
        private readonly IReviewService reviewService;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUserService;

        public GetAllMyReviewsAsUserQueryHandler(IReviewService reviewService, IMapper mapper, ICurrentUserService currentUserService)
        {
            this.reviewService = reviewService;
            this.mapper = mapper;
            this.currentUserService = currentUserService;
        }
        public async Task<
           Response<List<GetAllMyReviewsAsUserResult>>>
           Handle(
               GetAllMyReviewsAsUserQuery request,
               CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            var reviews =
                await reviewService.GetAllMyReviewAsUser(
                    userId,
                    cancellationToken);

            var result =
                mapper.Map<List<GetAllMyReviewsAsUserResult>>(
                    reviews);

            return Success(result);
        }

    }
}
