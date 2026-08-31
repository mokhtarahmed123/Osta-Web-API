using MediatR;
using Osta.Payment.Model;

namespace Osta.Core.Feature.Payment.Command
{
    public record CreatePaymentIntentCommand(int BookingId, decimal Amount, string? CouponCode)
        : IRequest<PaymentIntentResult>;
}
