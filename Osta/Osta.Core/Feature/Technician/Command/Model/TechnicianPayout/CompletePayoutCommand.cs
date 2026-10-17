using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Technician.Command.Model.TechnicianPayout
{
    public record CompletePayoutCommand(int Payout) : IRequest<Response<string>>
 ;
}
