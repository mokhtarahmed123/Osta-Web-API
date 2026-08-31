using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianServiceArea;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianServiceAreaCommandHandler
{
    public class UpdateTechnicianServiceAreaCommandHandler : ResponseHandler, IRequestHandler<UpdateTechnicianServiceAreaCommand, Response<string>>
    {
        private readonly ITechnicianServiceAreasService technicianServiceAreasService;
        private readonly ILoggerService loggerService;
        private readonly ICurrentUserService currentUser;

        public UpdateTechnicianServiceAreaCommandHandler(ITechnicianServiceAreasService technicianServiceAreasService, ILoggerService loggerService, ICurrentUserService currentUser) : base(technicianServiceAreasService, loggerService)
        {
            this.currentUser = currentUser;
            this.technicianServiceAreasService = technicianServiceAreasService;
            this.loggerService = loggerService;

        }
        public async Task<Response<string>> Handle(UpdateTechnicianServiceAreaCommand request, CancellationToken cancellationToken)
        {
            var TechId = currentUser.UserId;
            try
            {
                var technicianHasThisServiceArea = await technicianServiceAreasService
                    .TechnicianHasThisServiceAreaAsync(TechId, request.OldServiceAreaId, cancellationToken);

                if (!technicianHasThisServiceArea)
                {
                    return NotFound<string>(
                        $"This technician is not assigned to Service Area with Id {request.OldServiceAreaId}.");
                }

                var technicianServiceArea = new Data.Entities.Technician.TechnicianServiceArea
                {
                    TechnicianId = TechId,
                    ServiceAreaId = request.newServiceAreaId
                };

                await technicianServiceAreasService.ChangeTechnicianServiceAreaAsync(request.OldServiceAreaId, technicianServiceArea, cancellationToken);

                loggerService.LogInformation(
                    $"Technician service area updated successfully. TechnicianId: {TechId}, OldServiceAreaId: {request.OldServiceAreaId}, NewServiceAreaId: {request.newServiceAreaId}");

                return Updated<string>("Technician service area updated successfully.");
            }
            catch (Exception ex)
            {
                loggerService.LogError(
                    $"Error updating technician service area. TechnicianId: {TechId}, OldServiceAreaId: {request.OldServiceAreaId}, NewServiceAreaId: {request.newServiceAreaId}. Error: {ex.Message}");

                return BadRequest<string>(
                    "An error occurred while updating the technician service area.");
            }
        }
    }
}
