using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianAvailabilities;

namespace Osta.Core.Feature.Technician.Query.Model.ModelTechnicianAvailabilities
{
    public record GetAllTechnicianAvailabilitiesQuery() : IRequest<Response<List<GetAllTechnicianAvailabilitiesResult>>>;
}
