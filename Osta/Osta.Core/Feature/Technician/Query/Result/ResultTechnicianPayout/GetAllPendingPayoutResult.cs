using Osta.Domain.Enum;

namespace Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout
{
    public record GetAllPendingPayoutResult(int Id,
        string TechnicianId,
        decimal Amount,
        PayoutStatus Status,
        DateTime RequestedAt)
    {
    }
}
