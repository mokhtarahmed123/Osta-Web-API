using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianPayout;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianPayoutCommandHandler
{
    public class RequestPayoutCommandHandler
        : ResponseHandler,
          IRequestHandler<RequestPayoutCommand, Response<string>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianPayoutService _technicianPayoutService;
        private readonly ITechnicianWalletService _technicianWalletService;

        public RequestPayoutCommandHandler(
            ICurrentUserService currentUserService,
            ITechnicianPayoutService technicianPayoutService,
            ITechnicianWalletService technicianWalletService)
        {
            _currentUserService = currentUserService;
            _technicianPayoutService = technicianPayoutService;
            _technicianWalletService = technicianWalletService;
        }

        public async Task<Response<string>> Handle(
            RequestPayoutCommand request,
            CancellationToken cancellationToken)
        {

            var technicianId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(technicianId))
                return Unauthorized<string>("User is not authenticated.");

            var wallet = await _technicianWalletService
                .GetWalletAsync(
                    technicianId,
                    cancellationToken);

            if (wallet is null)
                return NotFound<string>(
                    "Technician wallet was not found.");

            if (wallet.Amount < request.Amount)
                return BadRequest<string>(
                    "Insufficient wallet balance.");

            var pendingPayouts =
              await _technicianPayoutService
          .GetTechnicianPayoutsAsync(
              technicianId,
              cancellationToken);

            var hasPendingSameAmount = pendingPayouts.Any(x =>
                x.Status == Osta.Domain.Enum.PayoutStatus.Pending &&
                x.Amount == request.Amount);

            if (hasPendingSameAmount)
            {
                return BadRequest<string>(
                    "You already have a pending payout with the same amount.");
            }

            var payout = await _technicianPayoutService
                .RequestPayoutAsync(
                    technicianId,
                    request.Amount,
                    request.Method,
                    request.ReceivingDetails,
                    cancellationToken);

            return Success(
                $"Payout request #{payout.Id} created successfully.");
        }
    }
}