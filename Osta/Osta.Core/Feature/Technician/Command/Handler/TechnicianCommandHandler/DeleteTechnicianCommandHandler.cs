using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianCommandHandler
{
    public class DeleteTechnicianCommandHandler : ResponseHandler, IRequestHandler<DeleteTechnicianCommand, Response<string>>
    {

        private readonly ITechnicianService technicianService;
        private readonly ILoggerService loggerService;
        private readonly ITechnicianServiceService technicianServiceService;
        private readonly ITechnicianServiceAreasService technicianServiceAreasService;
        private readonly ITechnicianImagesService technicianImagesService;


        public DeleteTechnicianCommandHandler(ITechnicianService technicianService, ILoggerService loggerService, ITechnicianServiceService technicianServiceService, ITechnicianServiceAreasService technicianServiceAreasService, ITechnicianImagesService technicianImagesService)
        {

            this.technicianService = technicianService;
            this.loggerService = loggerService;
            this.technicianServiceService = technicianServiceService;
            this.technicianServiceAreasService = technicianServiceAreasService;
            this.technicianImagesService = technicianImagesService;

        }

        public async Task<Response<string>> Handle(DeleteTechnicianCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var technician = await technicianService.GetTechnicianAsync(request.technicianId);
                if (technician is null)
                {
                    loggerService.LogError($"Technician With Id {request.technicianId} Not Found");
                    return NotFound<string>($"Technician With Id {request.technicianId} Not Found");
                }

                loggerService.LogInformation($"Deleting All Service TO Technician with ID {request.technicianId}");
                await technicianServiceService.DeleteAllService_technicianBy_technicianIdAsync(request.technicianId, cancellationToken);
                loggerService.LogInformation($"All Service with Technician ID {request.technicianId} deleted successfully");

                loggerService.LogInformation($"Deleting All Service Area TO Technician with ID {request.technicianId}");
                await technicianServiceAreasService.DeleteAllTechnicianServiceAreasWithSpecifyTechnicianIdAsync(request.technicianId, cancellationToken);
                loggerService.LogInformation($"All Service Area with Technician ID {request.technicianId} deleted successfully");

                // حذف الصور المرتبطة بالفني
                loggerService.LogInformation($"Deleting images for Technician with ID {request.technicianId}");
                await technicianImagesService.Delete(request.technicianId);
                loggerService.LogInformation($"Images for Technician with ID {request.technicianId} deleted successfully");

                await technicianService.DeleteTechnicianAsync(request.technicianId);

                loggerService.LogInformation($"Technician with ID {request.technicianId} deleted successfully");

                return Deleted<string>("Technician deleted successfully.");
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, $"Error occurred while deleting Technician with ID {request.technicianId}");
                return BadRequest<string>("An error occurred while processing your request.");
            }
        }

    }
}
