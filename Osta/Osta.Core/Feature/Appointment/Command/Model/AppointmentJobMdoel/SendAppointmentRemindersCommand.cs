using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Appointment.Command.Model.AppointmentJobMdoel
{
    public record SendAppointmentRemindersCommand : IRequest<Response<int>>;
}
