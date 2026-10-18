using MediatR;
using Osta.Core.Bases;
using Osta.Data.Enum;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Payment.Services;

namespace Osta.Core.Feature.Payment.Command
{
    public class RefundPaymentCommandHandler : ResponseHandler, IRequestHandler<RefundPaymentCommand, Response<string>>
    {
        private readonly IPaymentService _paymentService;

        public RefundPaymentCommandHandler(
            IPaymentService paymentService,

            IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
        }
        public async Task<Response<string>> Handle(RefundPaymentCommand request, CancellationToken ct)
        {

            if (request.PaymentId < 0)
                return BadRequest<string>("Invalid payment Id.");
            var payment = await _paymentService.GetByIdAsync(request.PaymentId, ct);

            if (payment is null)
                return NotFound<string>("Payment not found.");

            if (payment.Status != PaymentStatus.Completed)
                return BadRequest<string>("Only completed payments can be refunded.");

            var refundId = await _paymentService.RefundPaymentAsync(payment.TransactionId, ct);

            payment.Status = PaymentStatus.Refunded;
            await _paymentService.UpdateAsync(payment, ct);


            return Success<string>($"Refund initiated successfully. Refund Id: {refundId}");
        }
    }
}
