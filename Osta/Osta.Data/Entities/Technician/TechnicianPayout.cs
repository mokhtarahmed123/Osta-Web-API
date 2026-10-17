using Osta.Data.Entities.Technician;
using Osta.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Domain.Entities.Technician
{
    [Table("TechnicianPayout", Schema = "Technician")]
    public class TechnicianPayout
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Technician))]
        public string TechnicianId { get; set; }

        public decimal Amount { get; set; }

        public PayoutStatus Status { get; set; }

        public DateTime RequestedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string? RejectionReason { get; set; }
        public Technicians Technician { get; set; }


        public PayoutMethod Method { get; set; }
        public string ReceivingDetails { get; set; } = null!;


    }
}
