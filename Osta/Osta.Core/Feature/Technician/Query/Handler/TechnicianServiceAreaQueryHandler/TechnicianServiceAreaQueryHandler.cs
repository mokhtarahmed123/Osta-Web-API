using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianServiceArea;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianServiceArea;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Query.Handler.TechnicianServiceAreaQueryHandler
{
    public class TechnicianServiceAreaQueryHandler : ResponseHandler, IRequestHandler<GetAllTechniciansWithServiceAreaIdQuery, Response<List<GetAllTechniciansWithServiceAreaIdResult>>>
    {
        private readonly IMapper mapper;
        private readonly ILoggerService loggerService;
        private readonly ITechnicianService technicianService;
        private readonly ITechnicianServiceService technicianServiceService;
        private readonly ITechnicianServiceAreasService technicianServiceAreasService;
        private readonly IServiceService serviceService;

        public TechnicianServiceAreaQueryHandler(IMapper mapper, ILoggerService loggerService, ITechnicianService technicianService, ITechnicianServiceService technicianServiceService, ITechnicianServiceAreasService technicianServiceAreasService, IServiceService serviceService)
        {
            this.mapper = mapper;
            this.loggerService = loggerService;
            this.technicianService = technicianService;
            this.technicianServiceService = technicianServiceService;
            this.technicianServiceAreasService = technicianServiceAreasService;
            this.serviceService = serviceService;
        }
        public async Task<Response<List<GetAllTechniciansWithServiceAreaIdResult>>> Handle(GetAllTechniciansWithServiceAreaIdQuery request, CancellationToken cancellationToken)
        {
            var ServiceAreaIsFound = await technicianServiceAreasService.ServiceAreaHasTechniciansAsync(request.ServiceAreaId, cancellationToken);
            if (!ServiceAreaIsFound)
            {
                loggerService.LogError($" Area With Id {request.ServiceAreaId} Not found ");
                return NotFound<List<GetAllTechniciansWithServiceAreaIdResult>>("Not Found");
            }
            var technicians = await technicianService.GetTechniciansByServiceAreaIdAsync(request.ServiceAreaId);
            var Mapping = mapper.Map<List<GetAllTechniciansWithServiceAreaIdResult>>(technicians);

            return Success(Mapping);
        }

    }
}
