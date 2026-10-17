using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Appointment.Command.Model
{
    public record RejectAppointmentCommand(string AppointmentId, string Reason) : IRequest<Response<string>>
 ;
}
