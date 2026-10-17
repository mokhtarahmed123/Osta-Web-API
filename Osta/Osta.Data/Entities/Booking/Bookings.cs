using Osta.Data.Entities.Administration;
using Osta.Data.Entities.Identity;
using Osta.Data.Entities.Technician;
using Osta.Data.Enum;
using Osta.Domain.Entities.Appointment;
using Osta.Domain.Entities.Technician;

namespace Osta.Data.Entities.Booking
{

    public class Bookings
    {

        public int Id { get; set; }

        public string CustomerId { get; set; }

        public User Customer { get; set; } = null!;

        public string TechnicianId { get; set; } = string.Empty;

        public Technicians Technician { get; set; } = null!;


        public string Street { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;




        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public BookingStatus Status { get; set; } = BookingStatus.Pending;




        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Media> Medias { get; set; } = new List<Media>();
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public ICollection<TechnicianEarning> TechnicianEarnings { get; set; } = new List<TechnicianEarning>();
        public Appointment Appointment { get; set; } = null!;

    }
}
