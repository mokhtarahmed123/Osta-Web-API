using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnician;

namespace Osta.Core.Feature.Technician.Query.Model.TechnicianModel
{
    public record GetTechnicianByIdQuery(string TechnicianId) : IRequest<Response<GetTechnicianByIdResult>>
    ;

}
