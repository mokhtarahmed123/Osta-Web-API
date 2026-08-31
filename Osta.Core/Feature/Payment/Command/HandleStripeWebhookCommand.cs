using MediatR;

namespace Osta.Core.Feature.Payment.Command
{
    public record HandleStripeWebhookCommand(string Json, string Signature) : IRequest;

}
