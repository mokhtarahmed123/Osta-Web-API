using Osta.Payment.Model;

namespace Osta.Payment.Services;

public interface IPaymentQueryService
{
    Task<List<GetMyPaymentsResult>> GetMyPaymentsAsync(
        string userId,
        CancellationToken cancellationToken);
}