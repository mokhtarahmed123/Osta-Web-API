using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout;

namespace Osta.Core.Feature.Technician.Query.Model.ModelTechnicianPayout
{
    public record GetAllPendingPayoutQuery() : IRequest<Response<List<GetAllPendingPayoutResult>>>;
}
