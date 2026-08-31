using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.MediaBooking.Command.Model
{
    public record DeleteMediaBookingCommand(int Id) : IRequest<Response<string>>
    ;

}
