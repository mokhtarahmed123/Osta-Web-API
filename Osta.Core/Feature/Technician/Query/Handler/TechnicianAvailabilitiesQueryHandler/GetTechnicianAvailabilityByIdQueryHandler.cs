using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Model.ModelTechnicianAvailabilities;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianAvailabilities;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Query.Handler.TechnicianAvailabilitiesQueryHandler
{
    public class GetTechnicianAvailabilityByIdQueryHandler : ResponseHandler, IRequestHandler<GetTechnicianAvailabilityByIdQuery, Response<GetTechnicianAvailabilityByIdResult>>

    {
        private readonly IMapper mapper;
        private readonly ILoggerService loggerService;
        private readonly ITechnicianAvailabilityService technicianAvailabilityService;

        public GetTechnicianAvailabilityByIdQueryHandler(IMapper mapper, ILoggerService loggerService, ITechnicianAvailabilityService technicianAvailabilityService)
        {
            this.mapper = mapper;
            this.loggerService = loggerService;
            this.technicianAvailabilityService = technicianAvailabilityService;
        }
        public async Task<Response<GetTechnicianAvailabilityByIdResult>> Handle(
            GetTechnicianAvailabilityByIdQuery request,
            CancellationToken cancellationToken)
        {
            loggerService.LogInformation(
                "Getting technician availability with ID {AvailabilityId}",
                request.Id);

            var availability = await technicianAvailabilityService
                .GetTechnicianAvailabilityAsync(
                    request.Id,
                    cancellationToken);

            if (availability is null)
            {
                loggerService.LogWarning(
                    "Technician availability with ID {AvailabilityId} was not found",
                    request.Id);

                return NotFound<GetTechnicianAvailabilityByIdResult>(
                    "Technician availability not found.");
            }

            var result = mapper.Map<GetTechnicianAvailabilityByIdResult>(availability);

            loggerService.LogInformation(
                "Technician availability with ID {AvailabilityId} retrieved successfully",
                request.Id);

            return Success(result);
        }

    }
}
