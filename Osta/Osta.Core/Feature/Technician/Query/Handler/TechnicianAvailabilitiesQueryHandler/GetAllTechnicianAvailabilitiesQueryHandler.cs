using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianAvailabilities;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianAvailabilities;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Query.Handler.TechnicianAvailabilitiesQueryHandler
{
    public class GetAllTechnicianAvailabilitiesQueryHandler : ResponseHandler, IRequestHandler<GetAllTechnicianAvailabilitiesQuery, Response<List<GetAllTechnicianAvailabilitiesResult>>>
    {
        private readonly IMapper mapper;
        private readonly ILoggerService loggerService;
        private readonly ITechnicianAvailabilityService technicianAvailabilityService;

        public GetAllTechnicianAvailabilitiesQueryHandler(IMapper mapper, ILoggerService loggerService, ITechnicianAvailabilityService technicianAvailabilityService)
        {
            this.mapper = mapper;
            this.loggerService = loggerService;
            this.technicianAvailabilityService = technicianAvailabilityService;
        }

        public async Task<Response<List<GetAllTechnicianAvailabilitiesResult>>> Handle(
            GetAllTechnicianAvailabilitiesQuery request,
            CancellationToken cancellationToken)
        {
            loggerService.LogInformation("Getting all technician availabilities");

            var availabilities = await technicianAvailabilityService
                .GetAllTechnicianAvailabilitiesAsync(cancellationToken);

            if (!availabilities.Any())
            {
                loggerService.LogWarning("No technician availabilities found.");

                return NotFound<List<GetAllTechnicianAvailabilitiesResult>>(
                    "No technician availabilities found.");
            }

            var result = mapper.Map<List<GetAllTechnicianAvailabilitiesResult>>(availabilities);

            loggerService.LogInformation(
                "{Count} technician availabilities retrieved successfully.",
                result.Count);

            return Success(result, $"Count = {result.Count}");
        }
    }
}
