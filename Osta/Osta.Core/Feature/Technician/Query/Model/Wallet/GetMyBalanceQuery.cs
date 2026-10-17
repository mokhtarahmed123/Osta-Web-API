
using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Technician.Query.Model.Wallet
{
    public record GetMyBalanceQuery() : IRequest<Response<decimal>>;
}
