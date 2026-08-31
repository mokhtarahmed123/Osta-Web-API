using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Technician.Command.Model.TechnicianModel
{
    public record RejectTechnicianCommand(string TechId)
      : IRequest<Response<string>>
    {
        public string Reason { get; init; } = string.Empty;
    }
}
