using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Service.Command.Model;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;
using System.Diagnostics;

namespace Osta.Core.Feature.Service.Command.Handler
{
    public class UpdateServiceCommandHandler : ResponseHandler, IRequestHandler<UpdateServiceCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly IServiceService serviceService;
        private readonly ILoggerService loggerService;

        public UpdateServiceCommandHandler(IMapper mapper, IServiceService serviceService, ILoggerService loggerService)

        {
            this.mapper = mapper;
            this.serviceService = serviceService;
            this.loggerService = loggerService;
        }
        public async Task<Response<string>> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingService = await serviceService.GetServiceAsync(request.Id);
                if (existingService == null)
                {
                    loggerService.LogWarning("Attempted to update non-existent Service {ServiceId}", request.Id);
                    return NotFound<string>("Service not found.");
                }

                var sw = Stopwatch.StartNew();
                loggerService.LogInformation("Updating Service {ServiceId}", request.Id);

                var service = mapper.Map<Data.Entities.Services.Service>(request);
                await serviceService.UpdateServiceAsync(request.Id, service, request.Image, cancellationToken);

                sw.Stop();
                loggerService.LogInformation("Handler took {Elapsed} ms", sw.ElapsedMilliseconds);
                loggerService.LogInformation("Service {ServiceId} updated successfully", request.Id);

                return Updated<string>("Service updated successfully.");
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "Error occurred while updating Service {ServiceId}", request.Id);
                return BadRequest<string>("An error occurred while processing your request.");
            }
        }
    }
}
