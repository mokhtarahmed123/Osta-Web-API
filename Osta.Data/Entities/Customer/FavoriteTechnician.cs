using Osta.Data.Entities.Identity;
using Osta.Data.Entities.Technician;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities
{
    [Table("FavoriteTechnicians", Schema = "Customer")]

    public class FavoriteTechnician
    {
        public string CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public User Customer { get; set; } = null!;
        public string TechnicianId { get; set; } = string.Empty;

        [ForeignKey(nameof(TechnicianId))]
        public Technicians Technician { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



    }
}
