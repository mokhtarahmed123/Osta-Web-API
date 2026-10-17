using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Service.Command.Model;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;
using System.Diagnostics;

namespace Osta.Core.Feature.Service.Command.Handler
{
    public class AddServiceCommandHandler : ResponseHandler, IRequestHandler<AddServiceCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly IServiceService serviceService;
        private readonly ILoggerService loggerService;

        public AddServiceCommandHandler(IMapper mapper, IServiceService serviceService, ILoggerService loggerService)
        {
            this.mapper = mapper;
            this.serviceService = serviceService;
            this.loggerService = loggerService;
        }

        public async Task<Response<string>> Handle(AddServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                loggerService.LogInformation("Adding new Service with name {Service}", request.Name);
                var service = mapper.Map<Data.Entities.Services.Service>(request);
                await serviceService.AddServiceAsync(service, request.Image, cancellationToken);
                sw.Stop();
                loggerService.LogInformation("Handler took {Elapsed}  ms", sw.ElapsedMilliseconds); // Time taken to handle the request 
                loggerService.LogInformation("Service {ServiceId} created successfully", service.Id);

                return Created<string>("Service created successfully.");
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "Error occurred while adding Service with name {ServiceName}", request.Name);
                return BadRequest<string>("An error occurred while processing your request.");
            }


        }
    }
}
