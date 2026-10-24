namespace Osta.Data.Entities.Technician
{
    public class TechnicianAvailability
    {
        public int Id { get; set; }
        public string TechnicianId { get; set; } = string.Empty;
        public Technicians Technician { get; set; } = null!;
        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}