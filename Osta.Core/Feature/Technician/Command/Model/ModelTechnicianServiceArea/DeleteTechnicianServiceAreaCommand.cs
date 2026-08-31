using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Technician.Command.Model.ModelTechnicianServiceArea
{
    public record DeleteTechnicianServiceAreaCommand(int ServiceAreaId) : IRequest<Response<string>>;

}
