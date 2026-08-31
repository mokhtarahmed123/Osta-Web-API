using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianPayout;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianPayoutCommandHandler
{
    public class RejectPayoutCommandHandler
        : ResponseHandler,
          IRequestHandler<RejectPayoutCommand, Response<string>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianPayoutService _technicianPayoutService;

        public RejectPayoutCommandHandler(
            ICurrentUserService currentUserService,
            ITechnicianPayoutService technicianPayoutService)
        {
            _currentUserService = currentUserService;
            _technicianPayoutService = technicianPayoutService;
        }

        public async Task<Response<string>> Handle(
            RejectPayoutCommand request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized<string>(
                    "User is not authenticated.");

            if (request.PayoutId <= 0)
                return BadRequest<string>(
                    "Invalid payout id.");

            if (string.IsNullOrWhiteSpace(request.RejectionReason))
                return BadRequest<string>(
                    "Rejection reason is required.");

            var result = await _technicianPayoutService.RejectPayoutAsync(
                request.PayoutId,
                request.RejectionReason,
                cancellationToken);

            if (!result)
                return NotFound<string>(
                    "Payout not found.");

            return Success(
                "Payout rejected successfully.");
        }
    }
}