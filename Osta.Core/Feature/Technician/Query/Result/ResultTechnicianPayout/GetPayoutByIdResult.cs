using Osta.Domain.Enum;

namespace Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout
{
    public record GetPayoutByIdResult(
        int Id,
        decimal Amount,
        PayoutStatus Status,
        DateTime RequestedAt,
        DateTime? CompletedAt,
        string? RejectionReason
    );
}