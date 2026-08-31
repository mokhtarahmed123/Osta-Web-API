using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Technician.Command.Model.TechnicianPayout
{
    public record RejectPayoutCommand(int PayoutId, string RejectionReason) : IRequest<Response<string>>
   ;
}
