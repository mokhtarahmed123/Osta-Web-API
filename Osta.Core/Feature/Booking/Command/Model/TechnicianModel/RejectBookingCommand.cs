using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Booking.Command.Model.TechnicianModel
{
    public record RejectBookingCommand(int BookingId) : IRequest<Response<string>>;

}
