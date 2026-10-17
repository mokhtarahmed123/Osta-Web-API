using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianAvailabilities;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianAvailabilitiesCommandHandler
{
    public class DeleteTechnicianAvailabilityCommandHandler : ResponseHandler, IRequestHandler<DeleteTechnicianAvailabilityCommand, Response<string>>
    {

        private readonly ILoggerService loggerService;
        private readonly ITechnicianAvailabilityService technicianAvailabilityService;
        private readonly ICurrentUserService currentUser;

        public DeleteTechnicianAvailabilityCommandHandler(ILoggerService loggerService, ITechnicianAvailabilityService technicianAvailabilityService, ICurrentUserService currentUser)
        {

            this.loggerService = loggerService;
            this.technicianAvailabilityService = technicianAvailabilityService;
            this.currentUser = currentUser;
        }

        public async Task<Response<string>> Handle(
            DeleteTechnicianAvailabilityCommand request,
            CancellationToken cancellationToken)
        {
            var TechId = currentUser.UserId;
            loggerService.LogInformation(
                "Deleting availability {AvailabilityId} for Technician {TechnicianId}",
                request.Id,
                TechId);

            var availability = await technicianAvailabilityService.GetTechnicianAvailabilityForTechnicianAsync(
                request.Id, TechId,
                cancellationToken);

            if (availability is null)
            {
                loggerService.LogWarning(
                    "Availability with ID {AvailabilityId} was not found",
                    request.Id);

                return NotFound<string>("Availability not found.");
            }

            await technicianAvailabilityService.DeleteTechnicianAvailabilityAsync(
                request.Id,
                TechId,
                cancellationToken);

            loggerService.LogInformation(
                "Availability {AvailabilityId} deleted successfully",
                request.Id);

            return Deleted<string>("Availability deleted successfully.");
        }

    }
}
