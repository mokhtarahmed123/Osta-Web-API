using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.FavoriteTechnician.Query.Result;

namespace Osta.Core.Feature.FavoriteTechnician.Query.Model
{
    public record GetMyFavoriteQuery() : IRequest<Response<List<GetMyFavoriteResult>>>
 ;
}
