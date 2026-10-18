using Microsoft.Extensions.Options;
using Osta.Infrastructure.Abstract.PaymentAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Payment.Model;
using Osta.Payment.Settings;
using Stripe;

namespace Osta.Payment.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly StripeSettings _settings;
        private readonly PaymentIntentService _paymentIntentService = new();
        private readonly RefundService _refundService = new();
        private readonly IPaymentRepository paymentRepository;
        private readonly IUnitOfWork unitOfWork;

        public StripePaymentService(IOptions<StripeSettings> settings, IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
        {
            _settings = settings.Value;
            this.paymentRepository = paymentRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<PaymentIntentResult> CreatePaymentIntentAsync(
     decimal amount, string currency, int bookingId, string? couponCode, CancellationToken ct)
        {
            var metadata = new Dictionary<string, string>
    {
        { "bookingId", bookingId.ToString() }
    };

            if (!string.IsNullOrWhiteSpace(couponCode))
                metadata["CouponCode"] = couponCode;

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = currency,
                Metadata = metadata,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                    AllowRedirects = "never"
                }
            };

            var intent = await _paymentIntentService.CreateAsync(options, cancellationToken: ct);
            return new PaymentIntentResult(intent.ClientSecret, intent.Id);
        }

        public Event ConstructWebhookEvent(string json, string signatureHeader)
        {
            return EventUtility.ConstructEvent(json, signatureHeader, _settings.WebhookSecret, throwOnApiVersionMismatch: false);
        }

        public bool VerifyWebhookSignature(string receivedHmac, Dictionary<string, string> transactionData)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentIntentResult> UpdatePaymentIntentAmountAsync(
    string paymentIntentId,
    decimal amount,
    CancellationToken cancellationToken)
        {
            var service = new PaymentIntentService();

            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)(amount * 100)
            };

            var paymentIntent = await service.UpdateAsync(
                paymentIntentId,
                options,
                cancellationToken: cancellationToken);

            return new PaymentIntentResult(
                ClientSecret: paymentIntent.ClientSecret,
                PaymentIntentId: paymentIntent.Id);
        }

        public async Task<string> RefundPaymentAsync(string paymentIntentId, CancellationToken ct)
        {
            var options = new RefundCreateOptions
            {

                PaymentIntent = paymentIntentId
            };

            var refund = await _refundService.CreateAsync(options, cancellationToken: ct);
            return refund.Id;
        }

        public async Task<Data.Entities.Payment> GetByBookingIdAsync(int bookingId)
        {
            return await paymentRepository.GetByBookingIdAsync(bookingId);

        }

        public async Task UpdateAsync(Data.Entities.Payment payment, CancellationToken cancellationToken)
        {
            var existingPayment = await paymentRepository.GetByIdAsync(payment.Id, cancellationToken);
            existingPayment.Amount = payment.Amount;
            existingPayment.Status = payment.Status;
            existingPayment.Method = payment.Method;
            existingPayment.BookingId = payment.BookingId;
            existingPayment.TransactionId = payment.TransactionId;

            await paymentRepository.UpdateAsync(existingPayment, cancellationToken);
            await unitOfWork.SaveChangesAsync();

        }

        public async Task Add(Data.Entities.Payment payment, CancellationToken cancellationToken)
        {
            await paymentRepository.AddAsync(payment, cancellationToken);
            await unitOfWork.SaveChangesAsync();

        }

        public async Task<Data.Entities.Payment> GetByIdAsync(int ID, CancellationToken cancellationToken)
        {
            return await paymentRepository.GetByIdAsync(ID, cancellationToken);

        }

        public async Task<Data.Entities.Payment> GetByTransactionIdAsync(string ID, CancellationToken cancellationToken)
        {
            return await paymentRepository.GetByTransactionIdAsync(ID);

        }
    }
}
