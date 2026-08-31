using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianService;

namespace Osta.Core.Feature.Technician.Query.Model.ModelTechnicianService
{
    public record GetAllTechniciansWithServiceIdQuery(int ServiceId) : IRequest<Response<List<GetAllTechniciansWithServiceIdResult>>>;
}
