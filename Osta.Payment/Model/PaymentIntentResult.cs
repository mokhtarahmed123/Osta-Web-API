namespace Osta.Payment.Model
{
    public record PaymentIntentResult(string ClientSecret, string PaymentIntentId);
}
