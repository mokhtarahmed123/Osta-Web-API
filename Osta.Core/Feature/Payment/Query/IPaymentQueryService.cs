using Osta.Core.Feature.Payment.Query;

public interface IPaymentQueryService
{
    Task<List<GetMyPaymentsResult>> GetMyPaymentsAsync(
        string userId,
        CancellationToken cancellationToken);
}