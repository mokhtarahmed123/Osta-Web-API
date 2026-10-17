using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Service.Command.Model;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Service.Command.Handler
{
    public class DeleteServiceCommandHandler : ResponseHandler, IRequestHandler<DeleteServiceCommand, Response<string>>
    {

        private readonly IServiceService serviceService;
        private readonly ILoggerService loggerService;

        public DeleteServiceCommandHandler(IServiceService serviceService, ILoggerService loggerService)
        {

            this.serviceService = serviceService;
            this.loggerService = loggerService;
        }

        public async Task<Response<string>> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id <= 0)
                {
                    loggerService.LogWarning("Invalid Id {ServiceId}, Id must be greater than 0", request.Id);
                    return BadRequest<string>("Invalid service ID.");
                }

                var service = await serviceService.GetServiceAsync(request.Id);
                if (service == null)
                {
                    loggerService.LogWarning("Attempted to delete non-existent Service {ServiceId}", request.Id);
                    return NotFound<string>("Service not found.");
                }

                loggerService.LogInformation("Deleting service with ID {ServiceId}", request.Id);
                await serviceService.DeleteServiceAsync(request.Id, cancellationToken);
                loggerService.LogInformation("Service with ID {ServiceId} deleted successfully", request.Id);

                return Deleted<string>("Service deleted successfully.");
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "Error occurred while deleting service with ID {ServiceId}", request.Id);
                return BadRequest<string>("An error occurred while processing your request.");
            }
        }
    }
}
