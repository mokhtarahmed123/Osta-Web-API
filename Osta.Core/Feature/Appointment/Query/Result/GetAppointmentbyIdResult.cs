namespace Osta.Core.Feature.Appointment.Query.Result
{
    public record GetAppointmentbyIdResult
    {
        public required string Id { get; set; }
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public string? Notes { get; set; }
        public bool IsApproved { get; set; }
        public int BookingId { get; set; }
    }


}
