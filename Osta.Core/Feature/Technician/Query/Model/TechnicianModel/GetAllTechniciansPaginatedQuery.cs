using MediatR;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnician;
using Osta.Core.Wrappers;

namespace Osta.Core.Feature.Technician.Query.Model.TechnicianModel
{
    public record GetAllTechniciansPaginatedQuery(int PageNumber, int PageSize) : IRequest<PaginatedResult<GetAllTechniciansPaginatedResult>>;
}
