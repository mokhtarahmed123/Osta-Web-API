using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianPayout;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Technician.Query.Handler.TechnicianPayoutQueryHandler
{
    public class GetPayoutByIdQueryHandler
        : ResponseHandler,
          IRequestHandler<
              GetPayoutByIdQuery,
              Response<GetPayoutByIdResult>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianPayoutService _technicianPayoutService;
        private readonly IMapper _mapper;

        public GetPayoutByIdQueryHandler(
            ICurrentUserService currentUserService,
            ITechnicianPayoutService technicianPayoutService,
            IMapper mapper)
        {
            _currentUserService = currentUserService;
            _technicianPayoutService = technicianPayoutService;
            _mapper = mapper;
        }

        public async Task<Response<GetPayoutByIdResult>> Handle(
            GetPayoutByIdQuery request,
            CancellationToken cancellationToken)
        {
            var technicianId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(technicianId))
                return Unauthorized<GetPayoutByIdResult>(
                    "User is not authenticated.");

            if (request.PayoutId <= 0)
                return BadRequest<GetPayoutByIdResult>(
                    "Invalid payout id.");

            var payout = await _technicianPayoutService
                .GetPayoutByIdAsync(
                    request.PayoutId,
                    cancellationToken);

            if (payout is null)
                return NotFound<GetPayoutByIdResult>(
                    "Payout not found.");

            // Ownership check
            if (payout.TechnicianId != technicianId)
                return Unauthorized<GetPayoutByIdResult>(
                    "You are not allowed to access this payout.");

            var result = _mapper.Map<GetPayoutByIdResult>(payout);

            return Success(result);
        }
    }
}