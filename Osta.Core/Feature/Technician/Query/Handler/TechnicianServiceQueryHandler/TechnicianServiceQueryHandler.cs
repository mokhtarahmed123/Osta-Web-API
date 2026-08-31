.using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianService;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianService;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Query.Handler.TechnicianServiceQueryHandler
{
    public class TechnicianServiceQueryHandler : ResponseHandler, IRequestHandler<GetAllTechniciansWithServiceIdQuery, Response<List<GetAllTechniciansWithServiceIdResult>>>

    {

        private readonly IMapper mapper;
        private readonly ILoggerService loggerService;
        private readonly ITechnicianService technicianService;
        private readonly ITechnicianServiceService technicianServiceService;
        private readonly ITechnicianServiceAreasService technicianServiceAreasService;
        private readonly IServiceService serviceService;

        public TechnicianServiceQueryHandler(IMapper mapper, ILoggerService loggerService, ITechnicianService technicianService, ITechnicianServiceService technicianServiceService, ITechnicianServiceAreasService technicianServiceAreasService, IServiceService serviceService)
        {
            this.mapper = mapper;
            this.loggerService = loggerService;
            this.technicianService = technicianService;
            this.technicianServiceService = technicianServiceService;
            this.technicianServiceAreasService = technicianServiceAreasService;
            this.serviceService = serviceService;
        }

        public async Task<Response<List<GetAllTechniciansWithServiceIdResult>>> Handle(GetAllTechniciansWithServiceIdQuery request, CancellationToken cancellationToken)
        {
            var technicians = await technicianService.GetTechniciansByServiceIdAsync(request.ServiceId);
            if (!technicians.Any())
            {
                loggerService.LogWarning(
                    "No technicians found for Service Id {ServiceId}",
                    request.ServiceId);

                return NotFound<List<GetAllTechniciansWithServiceIdResult>>(
                    "No technicians found.");
            }
            var service = await serviceService.GetServiceAsync(request.ServiceId);
            var result = mapper.Map<List<GetAllTechniciansWithServiceIdResult>>(technicians);

            foreach (var technician in result)
            {
                technician.ServiceName = service.Name;
                technician.Price = (double)service.Price;
            }

            return Success(result);
        }

    }
}
