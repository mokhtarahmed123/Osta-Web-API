using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Data.Enum;
using Osta.Domain.Entities.Technician;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Technician
{
    [Table("Technicians", Schema = "Technician")]
    public class Technicians
    {
        [Key, ForeignKey(nameof(User))]
        public string Id { get; set; } = string.Empty;

        public User User { get; set; } = null!;

        [MaxLength(500)]
        public string? Bio { get; set; } = string.Empty;

        public bool IsVerified { get; set; } = false;

        [StringLength(14, MinimumLength = 14)]
        public string NationalId { get; set; } = string.Empty;
        public double Rating { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;

        public int CompletedBookings { get; set; } = 0;

        public int YearsOfExperience { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ReasonOfReject { get; set; }

        [MaxLength(20)]

        public StatusOfTechnicianRequestEnum Status { get; set; }
            = StatusOfTechnicianRequestEnum.None;

        public ICollection<TechnicianAvailability> Availabilities { get; set; } = new List<TechnicianAvailability>();
        public ICollection<TechnicianServiceArea> TechnicianServiceArea { get; set; } = new List<TechnicianServiceArea>();
        public ICollection<TechnicianService> TechnicianServices { get; set; } = new List<TechnicianService>();
        public ICollection<Bookings> Bookings { get; set; } = new List<Bookings>();
        public ICollection<FavoriteTechnician> FavoriteTechnicians { get; set; } = new List<FavoriteTechnician>();
        public ICollection<TechnicianEarning> TechnicianEarning { get; set; } = new List<TechnicianEarning>();
        public TechnicianWallet TechnicianWallet { get; set; }

        public ICollection<TechnicianPayout> TechnicianPayouts { get; set; }
    = new List<TechnicianPayout>();
    }
}
