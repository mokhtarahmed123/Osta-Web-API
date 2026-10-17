using MediatR;
using Osta.Core.Bases;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Payment.Query
{
    public class GetMyPaymentsQueryHandler
        : ResponseHandler,
          IRequestHandler<GetMyPaymentsQuery, Response<List<GetMyPaymentsResult>>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPaymentQueryService _paymentQueryService;

        public GetMyPaymentsQueryHandler(
            ICurrentUserService currentUserService,
            IPaymentQueryService paymentQueryService)
        {
            _currentUserService = currentUserService;
            _paymentQueryService = paymentQueryService;
        }

        public async Task<Response<List<GetMyPaymentsResult>>> Handle(
            GetMyPaymentsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest<List<GetMyPaymentsResult>>(
                    "User is not authenticated.");
            }

            var payments = await _paymentQueryService.GetMyPaymentsAsync(
                userId,
                cancellationToken);

            return Success(payments);
        }
    }
}