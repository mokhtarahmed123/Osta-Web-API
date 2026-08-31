using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianPayout;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout;
using Osta.Service.Abstract.TechnicianAbstract;

namespace Osta.Core.Feature.Technician.Query.Handler.TechnicianPayoutQueryHandler
{
    public class GetAllPendingPayoutQueryHandler
        : ResponseHandler,
          IRequestHandler<
              GetAllPendingPayoutQuery,
              Response<List<GetAllPendingPayoutResult>>>
    {
        private readonly ITechnicianPayoutService _technicianPayoutService;
        private readonly IMapper _mapper;

        public GetAllPendingPayoutQueryHandler(
            ITechnicianPayoutService technicianPayoutService,
            IMapper mapper)
        {
            _technicianPayoutService = technicianPayoutService;
            _mapper = mapper;
        }

        public async Task<Response<List<GetAllPendingPayoutResult>>> Handle(
            GetAllPendingPayoutQuery request,
            CancellationToken cancellationToken)
        {
            var payouts = await _technicianPayoutService
                .GetPendingPayoutsAsync(cancellationToken);

            var result = _mapper
                .Map<List<GetAllPendingPayoutResult>>(payouts);

            return Success(result);
        }
    }
}