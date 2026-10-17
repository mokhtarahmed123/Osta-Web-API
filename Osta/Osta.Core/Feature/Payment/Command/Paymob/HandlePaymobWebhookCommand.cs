using MediatR;

namespace Osta.Core.Feature.Payment.Command.Paymob
{
    public record HandlePaymobWebhookCommand(string ReceivedHmac, Dictionary<string, string> TransactionData) : IRequest;

}
