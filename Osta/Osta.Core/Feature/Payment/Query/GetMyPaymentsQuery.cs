using MediatR;
using Osta.Core.Bases;
using Osta.Payment.Model;

namespace Osta.Core.Feature.Payment.Query
{
    public record GetMyPaymentsQuery() : IRequest<Response<List<GetMyPaymentsResult>>>;

}
