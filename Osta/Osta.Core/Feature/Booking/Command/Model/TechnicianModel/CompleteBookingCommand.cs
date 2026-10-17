using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Booking.Command.Model.TechnicianModel
{
    public record CompleteBookingCommand(int Id) : IRequest<Response<string>>
;
}
