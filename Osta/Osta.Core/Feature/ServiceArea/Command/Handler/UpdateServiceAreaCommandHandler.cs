using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.ServiceArea.Command.Model;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.ServiceArea.Command.Handler
{
    public class UpdateServiceAreaCommandHandler : ResponseHandler, IRequestHandler<UpdateServiceAreaCommand, Response<string>>

    {
        private readonly IMapper mapper;
        private readonly ILoggerService loggerService;
        private readonly IServiceAreaService serviceAreaService;

        public UpdateServiceAreaCommandHandler(IMapper mapper, ILoggerService loggerService, IServiceAreaService serviceAreaService)
        {
            this.mapper = mapper;
            this.loggerService = loggerService;
            this.serviceAreaService = serviceAreaService;
        }

        public async Task<Response<string>> Handle(UpdateServiceAreaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var ServiceAreaexisting = await serviceAreaService.GetServiceAreaAsync(request.Id);
                if (ServiceAreaexisting == null)
                {
                    loggerService.LogError($"Service Area With Id {request.Id} Not Found");
                    return NotFound<string>($"Service Area With Id {request.Id} Not Found");
                }
                var ServiceArea = mapper.Map<Osta.Data.Entities.Technician.ServiceArea>(request);
                await serviceAreaService.UpdateServiceAreaAsync(request.Id, ServiceArea);

                loggerService.LogInformation($"Service Area updated successfully. Id: {request.Id}");
                return Updated<string>("Service Area updated successfully.");
            }
            catch (Exception ex)
            {
                loggerService.LogError($"Error updating Service Area with Id: {request.Id}. {ex.Message}");
                return BadRequest<string>("An error occurred while updating the Service Area.");
            }
        }
    }
}
