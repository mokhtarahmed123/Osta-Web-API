using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Model.Wallet;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Technician.Query.Handler.WalletHandler
{
    public class WalletQueryHandler
        : ResponseHandler,
          IRequestHandler<GetMyBalanceQuery, Response<decimal>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianWalletService _technicianWalletService;

        public WalletQueryHandler(
            ICurrentUserService currentUserService,
            ITechnicianWalletService technicianWalletService)
        {
            _currentUserService = currentUserService;
            _technicianWalletService = technicianWalletService;
        }

        public async Task<Response<decimal>> Handle(
            GetMyBalanceQuery request,
            CancellationToken cancellationToken)
        {
            var technicianId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(technicianId))
                return Unauthorized<decimal>(
                    "User is not authenticated.");

            var wallet = await _technicianWalletService
                .GetWalletAsync(
                    technicianId,
                    cancellationToken);

            if (wallet is null)
                return NotFound<decimal>(
                    "Technician wallet was not found.");

            return Success(wallet.Amount, $"Balance ={wallet.Amount}");
        }
    }
}