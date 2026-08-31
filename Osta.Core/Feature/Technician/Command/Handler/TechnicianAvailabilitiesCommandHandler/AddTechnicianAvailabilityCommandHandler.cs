using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianAvailabilities;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianAvailabilitiesCommandHandler
{
    public class AddTechnicianAvailabilityCommandHandler : ResponseHandler, IRequestHandler<RequestTechnicianAvailabilityCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly ILoggerService loggerService;
        private readonly ITechnicianAvailabilityService technicianAvailabilityService;
        private readonly ICurrentUserService currentUser;

        public AddTechnicianAvailabilityCommandHandler(IMapper mapper, ILoggerService loggerService, ITechnicianAvailabilityService technicianAvailabilityService, ICurrentUserService currentUser)
        {
            this.mapper = mapper;
            this.loggerService = loggerService;
            this.technicianAvailabilityService = technicianAvailabilityService;
            this.currentUser = currentUser;
        }

        public async Task<Response<string>> Handle(RequestTechnicianAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var techId = currentUser.UserId;
            try
            {

                loggerService.LogInformation(
                    "Adding availability for Technician {TechnicianId}",
                    techId);

                var availability = mapper.Map<TechnicianAvailability>(request);
                availability.TechnicianId = techId;

                await technicianAvailabilityService.AddTechnicianAvailabilityAsync(
                    availability,
                    cancellationToken);

                loggerService.LogInformation(
                    "Availability added successfully for Technician {TechnicianId}",
                    techId);

                return Created("Availability added successfully.");
            }
            catch (Exception ex)
            {
                loggerService.LogError(
                    ex,
                    $"Error while adding availability for Technician {techId}"
                        );

                return BadRequest<string>("Failed to add availability.");
            }
        }


    }
}
