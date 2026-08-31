using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.ServiceArea.Command.Model;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.ServiceArea.Command.Handler
{
    public class AddServiceAreaCommandHandler : ResponseHandler, IRequestHandler<AddServiceAreaCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly ILoggerService loggerService;
        private readonly IServiceAreaService serviceAreaService;

        public AddServiceAreaCommandHandler(IMapper mapper, ILoggerService loggerService, IServiceAreaService serviceAreaService)
        {
            this.mapper = mapper;
            this.loggerService = loggerService;
            this.serviceAreaService = serviceAreaService;
        }

        public async Task<Response<string>> Handle(AddServiceAreaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var ServiceArea = mapper.Map<Osta.Data.Entities.Technician.ServiceArea>(request);
                await serviceAreaService.AddServiceAreaAsync(ServiceArea);
                loggerService.LogInformation($"ServiceArea added successfully ");
                return Created<string>("ServiceArea added successfully.");

            }
            catch (Exception ex)
            {
                loggerService.LogError($"Error adding Service Area {ex.Message}");
                return BadRequest<string>("An error occurred while adding the Service Area.");
            }
        }
    }
}
