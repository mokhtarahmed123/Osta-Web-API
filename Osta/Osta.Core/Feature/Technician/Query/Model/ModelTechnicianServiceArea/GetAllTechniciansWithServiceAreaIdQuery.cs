using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianServiceArea;

namespace Osta.Core.Feature.Technician.Query.Model.ModelTechnicianServiceArea
{
    public record GetAllTechniciansWithServiceAreaIdQuery(int ServiceAreaId) : IRequest<Response<List<GetAllTechniciansWithServiceAreaIdResult>>>;
}
