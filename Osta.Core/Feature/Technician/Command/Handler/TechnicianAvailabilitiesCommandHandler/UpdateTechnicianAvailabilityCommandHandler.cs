using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianAvailabilities;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianAvailabilitiesCommandHandler
{
    public class UpdateTechnicianAvailabilityCommandHandler : ResponseHandler, IRequestHandler<UpdateTechnicianAvailabilityCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly ILoggerService loggerService;
        private readonly ITechnicianAvailabilityService technicianAvailabilityService;
        private readonly ICurrentUserService currentUser;

        public UpdateTechnicianAvailabilityCommandHandler(IMapper mapper, ILoggerService loggerService, ITechnicianAvailabilityService technicianAvailabilityService, ICurrentUserService currentUser)
        {
            this.mapper = mapper;
            this.loggerService = loggerService;
            this.technicianAvailabilityService = technicianAvailabilityService;
            this.currentUser = currentUser;
        }

        public async Task<Response<string>> Handle(
            UpdateTechnicianAvailabilityCommand request,
            CancellationToken cancellationToken)
        {
            var techId = currentUser.UserId;
            loggerService.LogInformation(
                "Updating technician availability {AvailabilityId} for Technician {TechnicianId}",
                request.Id,
                techId);

            var availability = await technicianAvailabilityService
                .GetTechnicianAvailabilityAsync(
                    request.Id,
                    cancellationToken);

            if (availability is null)
            {
                loggerService.LogWarning(
                    "Technician availability with ID {AvailabilityId} was not found",
                    request.Id);

                return NotFound<string>("Technician availability not found.");
            }

            mapper.Map(request, availability);
            availability.TechnicianId = techId;

            await technicianAvailabilityService.UpdateTechnicianAvailabilityAsync(
                request.Id,
             availability
           );

            loggerService.LogInformation(
                "Technician availability {AvailabilityId} updated successfully",
                request.Id);

            return Updated<string>("Technician availability updated successfully.");
        }
    }
}
