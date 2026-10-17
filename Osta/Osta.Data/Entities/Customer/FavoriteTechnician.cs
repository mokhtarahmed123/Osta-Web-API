using Osta.Data.Entities.Identity;
using Osta.Data.Entities.Technician;

namespace Osta.Data.Entities
{
    public class FavoriteTechnician
    {
        public string CustomerId { get; set; } = string.Empty;

        public User Customer { get; set; } = null!;
        public string TechnicianId { get; set; } = string.Empty;
        public Technicians Technician { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
