using Osta.Payment.Model;
using Stripe;

namespace Osta.Payment.Services
{
    public interface IPaymentService
    {
        Task<PaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string currency, int bookingId, string? couponCode, CancellationToken ct);
        bool VerifyWebhookSignature(string receivedHmac, Dictionary<string, string> transactionData);

        public Event ConstructWebhookEvent(string json, string signatureHeader);
        Task<string> RefundPaymentAsync(string paymentIntentId, CancellationToken ct);   // ← جديد

        Task<PaymentIntentResult> UpdatePaymentIntentAmountAsync(string paymentIntentId,
    decimal amount,
    CancellationToken cancellationToken);

        Task<Osta.Data.Entities.Payment> GetByBookingIdAsync(int bookingId);

        Task UpdateAsync(Osta.Data.Entities.Payment payment, CancellationToken cancellationToken);

        Task Add(Osta.Data.Entities.Payment payment, CancellationToken cancellationToken);
        Task<Osta.Data.Entities.Payment> GetByIdAsync(int ID, CancellationToken cancellationToken);
        Task<Osta.Data.Entities.Payment> GetByTransactionIdAsync(string ID, CancellationToken cancellationToken);

    }
}
