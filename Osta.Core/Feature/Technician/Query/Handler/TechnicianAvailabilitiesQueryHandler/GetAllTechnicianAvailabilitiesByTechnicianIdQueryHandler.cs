using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianAvailabilities;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianAvailabilities;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Query.Handler.TechnicianAvailabilitiesQueryHandler
{
    public class GetAllTechnicianAvailabilitiesByTechnicianIdQueryHandler : ResponseHandler, IRequestHandler<GetAllTechnicianAvailabilitiesByTechnicianIdQuery, Response<List<GetAllTechnicianAvailabilitiesByTechnicianIdResult>>>
    {
        private readonly IMapper mapper;
        private readonly ILoggerService loggerService;
        private readonly ITechnicianAvailabilityService technicianAvailabilityService;

        public GetAllTechnicianAvailabilitiesByTechnicianIdQueryHandler(IMapper mapper, ILoggerService loggerService, ITechnicianAvailabilityService technicianAvailabilityService)
        {
            this.mapper = mapper;
            this.loggerService = loggerService;
            this.technicianAvailabilityService = technicianAvailabilityService;
        }
        public async Task<Response<List<GetAllTechnicianAvailabilitiesByTechnicianIdResult>>> Handle(
            GetAllTechnicianAvailabilitiesByTechnicianIdQuery request,
            CancellationToken cancellationToken)
        {
            loggerService.LogInformation(
                "Getting availabilities for Technician {TechnicianId}",
                request);

            var availabilities = await technicianAvailabilityService
                .GetAllTechnicianAvailabilitiesByTechnicianIdAsync(
                        request.technicianId);

            if (!availabilities.Any())
            {
                loggerService.LogWarning(
                    "No availabilities found for Technician {TechnicianId}",
                    request.technicianId);

                return NotFound<List<GetAllTechnicianAvailabilitiesByTechnicianIdResult>>(
                    "No availabilities found.");
            }

            var result = mapper.Map<List<GetAllTechnicianAvailabilitiesByTechnicianIdResult>>(availabilities);

            loggerService.LogInformation(
                "Retrieved {Count} availabilities for Technician {TechnicianId}",
                result.Count,
                request.technicianId);

            return Success(result, $"Count = {result.Count}");
        }


    }
}
