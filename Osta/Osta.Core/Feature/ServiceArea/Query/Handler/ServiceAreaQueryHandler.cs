using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.ServiceArea.Query.Model;
using Osta.Core.Feature.ServiceArea.Query.Result;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.ServiceArea.Query.Handler
{
    public class ServiceAreaQueryHandler : ResponseHandler,
        IRequestHandler<GetAllServiceAreasQuery, Response<List<GetAllServiceAreasResult>>>,
        IRequestHandler<GetServiceAreaByIdQuery, Response<GetServiceAreaByIdResult>>
    {
        private readonly IMapper mapper;
        private readonly ILoggerService logger;
        private readonly IServiceAreaService serviceAreaService;

        public ServiceAreaQueryHandler(IMapper mapper, ILoggerService logger, IServiceAreaService serviceAreaService)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.serviceAreaService = serviceAreaService;
        }
        public async Task<Response<List<GetAllServiceAreasResult>>> Handle(GetAllServiceAreasQuery request, CancellationToken cancellationToken)
        {
            var ServiceAreas = await serviceAreaService.GetAllServiceAreasAsync();
            var result = mapper.Map<List<GetAllServiceAreasResult>>(ServiceAreas);
            return Success(result);

        }

        public async Task<Response<GetServiceAreaByIdResult>> Handle(GetServiceAreaByIdQuery request, CancellationToken cancellationToken)
        {
            var ServiceArea = await serviceAreaService.GetServiceAreaAsync(request.Id);

            if (ServiceArea is null)
            {
                logger.LogWarning($"Service Area not found with Id: {request.Id}");
                return NotFound<GetServiceAreaByIdResult>("Service Area not found.");
            }

            var result = mapper.Map<GetServiceAreaByIdResult>(ServiceArea);
            return Success(result);
        }
    }
}
