using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Payment.Command
{
    public record RefundPaymentCommand(int PaymentId) : IRequest<Response<string>>;
}
