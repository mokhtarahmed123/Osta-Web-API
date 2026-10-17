using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Review.Query.Model;
using Osta.Core.Feature.Review.Query.Result;
using Osta.Service.Abstract.ReviewAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Review.Query.Handler
{
    public class GetAllMyReviewsAsTechnicianQueryHandler : ResponseHandler,
        IRequestHandler<
            GetAllMyReviewsAsTechnicianQuery,
            Response<List<GetAllMyReviewsAsTechnicianResult>>>
    {
        private readonly IReviewService reviewService;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUserService;

        public GetAllMyReviewsAsTechnicianQueryHandler(IReviewService reviewService, IMapper mapper, ICurrentUserService currentUserService)

        {
            this.reviewService = reviewService;
            this.mapper = mapper;
            this.currentUserService = currentUserService;
        }


        public async Task<
            Response<List<GetAllMyReviewsAsTechnicianResult>>>
            Handle(
                GetAllMyReviewsAsTechnicianQuery request,
                CancellationToken cancellationToken)
        {
            var technicianId = currentUserService.UserId;

            if (string.IsNullOrEmpty(technicianId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            var reviews =
                await reviewService.GetAllMyReviewAsTechnician(
                    technicianId,
                    cancellationToken);

            var result =
                mapper.Map<List<GetAllMyReviewsAsTechnicianResult>>(
                    reviews);

            return Success(result);
        }

    }
}
