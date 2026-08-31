using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianServiceArea;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianServiceAreaCommandHandler
{
    public class AddTechnicianServiceAreaCommandHandler : ResponseHandler, IRequestHandler<AddTechnicianServiceAreaCommand, Response<string>>
    {


        private readonly ITechnicianServiceAreasService technicianServiceAreasService;
        private readonly ILoggerService loggerService;
        private readonly ICurrentUserService currentUser;

        public AddTechnicianServiceAreaCommandHandler(ITechnicianServiceAreasService technicianServiceAreasService, ILoggerService loggerService, ICurrentUserService currentUser)
        {
            this.technicianServiceAreasService = technicianServiceAreasService;
            this.loggerService = loggerService;
            this.currentUser = currentUser;
        }

        public ITechnicianServiceAreasService TechnicianServiceAreasService { get; }
        public ILoggerService LoggerService { get; }

        public async Task<Response<string>> Handle(AddTechnicianServiceAreaCommand request, CancellationToken cancellationToken)
        {
            var TechId = currentUser.UserId;
            try
            {

                var ThisTechnicianHasThisServiceArea = await technicianServiceAreasService.
                    TechnicianHasThisServiceAreaAsync(TechId, request.ServiceAreaId, cancellationToken
                    );
                if (ThisTechnicianHasThisServiceArea)
                {
                    return Conflict<string>("This technician is already assigned to this service area.");
                }
                var technicianServiceAreas = new Data.Entities.Technician.TechnicianServiceArea()
                {
                    ServiceAreaId = request.ServiceAreaId,
                    TechnicianId = TechId,

                };
                await technicianServiceAreasService.AddTechnicianServiceAreaAsync(technicianServiceAreas, cancellationToken);
                loggerService.LogInformation($"Technician service area added successfully. TechnicianId: {TechId}, ServiceAreaId: {request.ServiceAreaId}");
                return Created("Technician service area added successfully.");
            }
            catch (Exception ex)
            {
                loggerService.LogError($"Error adding technician service area. TechnicianId: {TechId}, ServiceAreaId: {request.ServiceAreaId}. Error: {ex.Message}");
                return BadRequest<string>("An error occurred while adding the technician service area.");
            }


        }

    }
}
