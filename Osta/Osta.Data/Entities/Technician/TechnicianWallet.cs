using Osta.Data.Entities.Technician;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Domain.Entities.Technician
{
    [Table("TechnicianWallet", Schema = "Technician")]
    public class TechnicianWallet
    {

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string TechnicianId { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;



        public Technicians Technician { get; set; } = null!;



    }
}