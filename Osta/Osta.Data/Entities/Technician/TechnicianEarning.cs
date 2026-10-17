using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Technician;

namespace Osta.Domain.Entities.Technician
{
    public class TechnicianEarning
    {
        public int Id { get; set; }
        public string TechnicianId { get; set; }
        public int BookingId { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
        public Bookings Booking { get; set; }
        public Technicians Technician { get; set; }

    }
}
