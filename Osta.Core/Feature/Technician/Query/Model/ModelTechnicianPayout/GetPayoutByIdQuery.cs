using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout;

namespace Osta.Core.Feature.Technician.Query.Model.ModelTechnicianPayout
{
    public record GetPayoutByIdQuery(int PayoutId) : IRequest<Response<GetPayoutByIdResult>>;
}
