using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Review.Command.Model
{
    public record DeleteReviewCommand(int Id) : IRequest<Response<string>>
;
}
