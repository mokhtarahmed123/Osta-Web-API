using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Abstract.PaymentAbstract
{
    public interface IPaymentRepository : IGenericRepositoryAsync<Osta.Data.Entities.Payment>
    {
        Task<Osta.Data.Entities.Payment> GetByTransactionIdAsync(string transactionId);
        Task<Osta.Data.Entities.Payment> GetByBookingIdAsync(int BookingId);
    }
}
