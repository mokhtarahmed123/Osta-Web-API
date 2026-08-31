using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Booking.Command.Model.CustomerModel
{
    public record CancelBookingCommand(int BookingId) : IRequest<Response<string>>;


}
