using MediatR;
using Osta.Core.Bases;
using Osta.Domain.Entities.Technician;

namespace Osta.Core.Feature.Technician.Command.Model.TechnicianPayout
{
    public record RequestPayoutCommand(decimal Amount, PayoutMethod Method,
        string ReceivingDetails) : IRequest<Response<string>>;


}
