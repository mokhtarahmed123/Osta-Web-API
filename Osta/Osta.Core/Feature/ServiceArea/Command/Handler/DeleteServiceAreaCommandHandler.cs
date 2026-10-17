using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.ServiceArea.Command.Model;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.ServiceArea.Command.Handler
{
    public class DeleteServiceAreaCommandHandler : ResponseHandler, IRequestHandler<DeleteServiceAreaCommand, Response<string>>
    {

        private readonly ILoggerService loggerService;
        private readonly IServiceAreaService serviceAreaService;

        public DeleteServiceAreaCommandHandler(ILoggerService loggerService, IServiceAreaService serviceAreaService)
        {

            this.loggerService = loggerService;
            this.serviceAreaService = serviceAreaService;
        }

        public async Task<Response<string>> Handle(DeleteServiceAreaCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var ServiceArea = await serviceAreaService.GetServiceAreaAsync(request.id);
                if (ServiceArea == null)
                {
                    loggerService.LogError($"Service Area   With Id {request.id} Not Found");
                    return NotFound<string>($"Service Area  With Id {request.id} Not Found");
                }

                await serviceAreaService.DeleteServiceAreaAsync(request.id);
                loggerService.LogInformation($"Service Area  deleted successfully. Id: {request.id}");
                return Deleted<string>("Service Area  deleted successfully.");
            }
            catch (Exception ex)
            {
                loggerService.LogError($"Error deleting Service Area  with Id: {request.id}. {ex.Message}");
                return BadRequest<string>("An error occurred while deleting the Service Area .");
            }
        }
    }
}
