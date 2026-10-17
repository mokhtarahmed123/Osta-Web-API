using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Technician.Command.Model.TechnicianModel
{
    public record TechnicianAddServiceCommand() : IRequest<Response<string>>
    {
        public List<int> ServiceIds { get; set; }
    };


}
