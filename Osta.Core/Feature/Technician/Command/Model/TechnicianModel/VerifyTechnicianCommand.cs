using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Technician.Command.Model.TechnicianModel
{
    public record VerifyTechnicianCommand(string TechId) : IRequest<Response<string>>;

}
