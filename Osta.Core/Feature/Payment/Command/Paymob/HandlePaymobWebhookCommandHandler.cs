using MediatR;
using Osta.Data.Enum;
using Osta.Infrastructure.Abstract.PaymentAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Payment.Services;

namespace Osta.Core.Feature.Payment.Command.Paymob
{
    internal class HandlePaymobWebhookCommandHandler : IRequestHandler<HandlePaymobWebhookCommand>
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public HandlePaymobWebhookCommandHandler(
            IPaymentService paymentService,
            IPaymentRepository paymentRepository,
            IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(HandlePaymobWebhookCommand request, CancellationToken cancellationToken)
        {
            var isValid = _paymentService.VerifyWebhookSignature(request.ReceivedHmac, request.TransactionData);
            if (!isValid)
                throw new UnauthorizedAccessException("Invalid Paymob HMAC signature");

            var transactionId = request.TransactionData.GetValueOrDefault("order"); // أو "id" حسب الحقل اللي بتخزنه
            var success = request.TransactionData.GetValueOrDefault("success") == "true";

            if (transactionId is null) return;

            var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId);
            if (payment is null) return;

            payment.Status = success ? PaymentStatus.Completed : PaymentStatus.Failed;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
