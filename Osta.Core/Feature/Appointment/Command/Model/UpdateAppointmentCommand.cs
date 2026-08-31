using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Appointment.Command.Model
{
    public record UpdateAppointmentCommand(string AppointmentId, int BookingId) : IRequest<Response<string>>  // Tech Will Update
    {
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public string? Notes { get; set; } = null;

    }
}
