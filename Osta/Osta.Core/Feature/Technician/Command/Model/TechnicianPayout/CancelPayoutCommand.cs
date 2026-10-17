using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Technician.Command.Model.TechnicianPayout
{
    public record CancelPayoutCommand(int PayoutId) : IRequest<Response<string>>
   ;
}
