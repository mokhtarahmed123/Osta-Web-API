using Osta.Data.Entities;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Abstract.PaymentAbstract
{
    public interface IPaymentRepository : IGenericRepositoryAsync<Payment>
    {
        Task<Payment> GetByTransactionIdAsync(string transactionId);
        Task<Payment> GetByBookingIdAsync(int BookingId);
    }
}
