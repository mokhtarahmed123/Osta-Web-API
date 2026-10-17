using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianPayout;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Technician.Query.Handler.TechnicianPayoutQueryHandler
{
    public class GetAllMyPayoutsQueryHandler
        : ResponseHandler,
          IRequestHandler<
              GetAllMyPayoutsQuery,
              Response<List<GetAllMyPayoutsResult>>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianPayoutService _technicianPayoutService;
        private readonly IMapper _mapper;

        public GetAllMyPayoutsQueryHandler(
            ICurrentUserService currentUserService,
            ITechnicianPayoutService technicianPayoutService,
            IMapper mapper)
        {
            _currentUserService = currentUserService;
            _technicianPayoutService = technicianPayoutService;
            _mapper = mapper;
        }

        public async Task<Response<List<GetAllMyPayoutsResult>>> Handle(
            GetAllMyPayoutsQuery request,
            CancellationToken cancellationToken)
        {
            var technicianId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(technicianId))
                return Unauthorized<List<GetAllMyPayoutsResult>>(
                    "User is not authenticated.");

            var payouts = await _technicianPayoutService
                .GetTechnicianPayoutsAsync(
                    technicianId,
                    cancellationToken);

            var result = _mapper
                .Map<List<GetAllMyPayoutsResult>>(payouts);

            return Success(result);
        }
    }
}