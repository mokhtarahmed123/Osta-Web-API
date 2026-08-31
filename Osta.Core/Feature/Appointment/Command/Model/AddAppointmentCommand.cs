using MediatR;
using Osta.Core.Bases;
using System.Text.Json.Serialization;

namespace Osta.Core.Feature.Appointment.Command.Model
{
    public record AddAppointmentCommand(int BookingId) : IRequest<Response<string>>  // Tech Will Add Appointment And Notify The Customer
    {
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public string? Notes { get; set; } = null;
        [JsonIgnore]
        public bool IsApproved { get; set; } = false;  // Customer Will Approved Or Not


    }
}
