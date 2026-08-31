using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Technician.Command.Model.ModelTechnicianAvailabilities
{
    public record DeleteTechnicianAvailabilityCommand(int Id
        ) : IRequest<Response<string>>
;
}
