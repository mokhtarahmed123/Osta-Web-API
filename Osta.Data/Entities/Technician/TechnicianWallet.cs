using Osta.Data.Entities.Technician;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Domain.Entities.Technician
{
    [Table("TechnicianWallet", Schema = "Technician")]
    public class TechnicianWallet
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string TechnicianId { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        [ForeignKey(nameof(TechnicianId))]
        public Technicians Technician { get; set; } = null!;



    }
}