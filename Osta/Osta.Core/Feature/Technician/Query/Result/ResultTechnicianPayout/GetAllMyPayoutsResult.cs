using Osta.Domain.Enum;

namespace Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout
{
    public record GetAllMyPayoutsResult
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public PayoutStatus Status { get; set; }

        public DateTime RequestedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string? RejectionReason { get; set; }
    }
}
