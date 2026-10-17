using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianServiceArea;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianServiceAreaCommandHandler
{
    public class DeleteTechnicianServiceAreaCommandHandler : ResponseHandler, IRequestHandler<DeleteTechnicianServiceAreaCommand, Response<string>>
    {
        private readonly ITechnicianServiceAreasService technicianServiceAreasService;
        private readonly ILoggerService loggerService;
        private readonly ICurrentUserService currentUser;

        public DeleteTechnicianServiceAreaCommandHandler(ITechnicianServiceAreasService technicianServiceAreasService, ILoggerService loggerService, ICurrentUserService currentUser)
        {
            this.technicianServiceAreasService = technicianServiceAreasService;
            this.loggerService = loggerService;
            this.currentUser = currentUser;
        }

        public async Task<Response<string>> Handle(DeleteTechnicianServiceAreaCommand request, CancellationToken cancellationToken)
        {
            var TechId = currentUser.UserId;
            try
            {
                var technicianHasThisServiceArea = await technicianServiceAreasService
                    .TechnicianHasThisServiceAreaAsync(TechId, request.ServiceAreaId, cancellationToken);

                if (!technicianHasThisServiceArea)
                {
                    return NotFound<string>(
                        $"This technician is not assigned to Service Area with Id {request.ServiceAreaId}.");
                }

                var technicianServiceArea = new Data.Entities.Technician.TechnicianServiceArea
                {
                    TechnicianId = TechId,
                    ServiceAreaId = request.ServiceAreaId
                };

                await technicianServiceAreasService.DeleteTechnicianServiceAreaAsync(technicianServiceArea, cancellationToken);

                loggerService.LogInformation(
                    $"Technician service area deleted successfully. TechnicianId: {TechId}, ServiceAreaId: {request.ServiceAreaId}");

                return Deleted<string>("Technician service area deleted successfully.");
            }
            catch (Exception ex)
            {
                loggerService.LogError(
                    $"Error deleting technician service area. TechnicianId: {TechId}, ServiceAreaId: {request.ServiceAreaId}. Error: {ex.Message}");

                return BadRequest<string>(
                    "An error occurred while deleting the technician service area.");
            }
        }
    }
}
