using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.FavoriteTechnician.Query.Model;
using Osta.Core.Feature.FavoriteTechnician.Query.Result;
using Osta.Service.Abstract.CustomerAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.FavoriteTechnician.Query.Handler
{
    public class FavoriteTechnicianQueryHandler :
        ResponseHandler,
        IRequestHandler<
            GetMyFavoriteQuery,
            Response<List<GetMyFavoriteResult>>>
    {
        private readonly IFavoriteTechnicianService favoriteTechnicianService;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public FavoriteTechnicianQueryHandler(
            IFavoriteTechnicianService favoriteTechnicianService,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            this.favoriteTechnicianService =
                favoriteTechnicianService;

            this.currentUserService =
                currentUserService;

            this.mapper = mapper;
        }

        public async Task<Response<List<GetMyFavoriteResult>>> Handle(
            GetMyFavoriteQuery request,
            CancellationToken cancellationToken)
        {
            var customerId =
                currentUserService.UserId;

            if (string.IsNullOrEmpty(customerId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            var favorites =
                await favoriteTechnicianService
                    .GetMyFavorites(customerId);

            var result =
                mapper.Map<List<GetMyFavoriteResult>>(
                    favorites);

            return Success(result);
        }
    }
}