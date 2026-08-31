using Osta.Data.Entities.Administration;
using Osta.Data.Entities.Identity;
using Osta.Data.Entities.Technician;
using Osta.Data.Enum;
using Osta.Domain.Entities.Appointment;
using Osta.Domain.Entities.Technician;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Booking
{
    [Table("Bookings", Schema = "Booking")]
    public class Bookings
    {
        [Key]
        public int Id { get; set; }

        public string CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public User Customer { get; set; } = null!;

        public string TechnicianId { get; set; } = string.Empty;
        [ForeignKey(nameof(TechnicianId))]
        public Technicians Technician { get; set; } = null!;


        public string Street { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;




        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public BookingStatus Status { get; set; } = BookingStatus.Pending;




        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Media> Media { get; set; } = new List<Media>();
        public ICollection<Complaint> Complaint { get; set; } = new List<Complaint>();
        public ICollection<TechnicianEarning> TechnicianEarning { get; set; } = new List<TechnicianEarning>();
        public Appointment Appointment { get; set; } = null!;

    }
}
