using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Technician.Command.Model.TechnicianModel
{
    public record DeleteTechnicianCommand(string technicianId) : IRequest<Response<string>>
 ;
}
