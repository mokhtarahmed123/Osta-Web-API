using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Technician;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Domain.Entities.Technician
{
    [Table("TechnicianEarning", Schema = "Technician")]
    public class TechnicianEarning
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey(nameof(Technician))]
        public string TechnicianId { get; set; }
        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;

        public Bookings Booking { get; set; }
        public Technicians Technician { get; set; }

    }
}
