using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Review.Query.Result;

namespace Osta.Core.Feature.Review.Query.Model
{
    public record GetReviewByIdQuery(int Id) : IRequest<Response<GetReviewByIdResult>>
;
}
