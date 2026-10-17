using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianPayout;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianPayoutCommandHandler
{
    public class CancelPayoutCommandHandler
        : ResponseHandler,
          IRequestHandler<CancelPayoutCommand, Response<string>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianPayoutService _technicianPayoutService;

        public CancelPayoutCommandHandler(
            ICurrentUserService currentUserService,
            ITechnicianPayoutService technicianPayoutService)
        {
            _currentUserService = currentUserService;
            _technicianPayoutService = technicianPayoutService;
        }

        public async Task<Response<string>> Handle(
            CancelPayoutCommand request,
            CancellationToken cancellationToken)
        {
            var technicianId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(technicianId))
                return Unauthorized<string>(
                    "User is not authenticated.");

            await _technicianPayoutService.CancelPayoutAsync(
                request.PayoutId,
                technicianId,
                cancellationToken);

            return Success(
                "Payout cancelled successfully.");
        }
    }
}