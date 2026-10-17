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


    }
}
