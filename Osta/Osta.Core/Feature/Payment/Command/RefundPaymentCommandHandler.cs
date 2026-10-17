using MediatR;
using Osta.Core.Bases;
using Osta.Data.Enum;
using Osta.Infrastructure.Abstract.PaymentAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Payment.Services;

namespace Osta.Core.Feature.Payment.Command
{
    public class RefundPaymentCommandHandler : ResponseHandler, IRequestHandler<RefundPaymentCommand, Response<string>>
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RefundPaymentCommandHandler(
            IPaymentService paymentService,
            IPaymentRepository paymentRepository,
            IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Response<string>> Handle(RefundPaymentCommand request, CancellationToken ct)
        {

            if (request.PaymentId < 0)
                return BadRequest<string>("Invalid payment Id.");
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, ct);

            if (payment is null)
                return NotFound<string>("Payment not found.");

            if (payment.Status != PaymentStatus.Completed)
                return BadRequest<string>("Only completed payments can be refunded.");

            var refundId = await _paymentService.RefundPaymentAsync(payment.TransactionId, ct);

            payment.Status = PaymentStatus.Refunded;
            await _paymentRepository.UpdateAsync(payment, ct);
            await _unitOfWork.SaveChangesAsync();

            return Success<string>($"Refund initiated successfully. Refund Id: {refundId}");
        }
    }
}
