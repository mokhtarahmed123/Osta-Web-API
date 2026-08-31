using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Technician.Command.Model.ModelTechnicianServiceArea
{
    public record UpdateTechnicianServiceAreaCommand(int OldServiceAreaId) : IRequest<Response<string>>

    {
        public int newServiceAreaId
        { get; set; }

    }


}
