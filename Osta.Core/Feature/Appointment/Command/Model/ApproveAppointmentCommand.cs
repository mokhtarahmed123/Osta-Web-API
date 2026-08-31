using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Appointment.Command.Model
{
    public record ApproveAppointmentCommand(string AppointmentId) : IRequest<Response<string>>;
}
