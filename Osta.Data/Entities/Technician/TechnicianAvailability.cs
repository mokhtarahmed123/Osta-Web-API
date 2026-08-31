using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Technician
{
    [Table("TechnicianAvailabilities", Schema = "Technician")]
    public class TechnicianAvailability
    {
        [Key]
        public int Id { get; set; }

        public string TechnicianId { get; set; } = string.Empty;

        [ForeignKey(nameof(TechnicianId))]
        public Technicians Technician { get; set; } = null!;

        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}