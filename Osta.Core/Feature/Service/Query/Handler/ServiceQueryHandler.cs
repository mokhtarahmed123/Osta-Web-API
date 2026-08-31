using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Service.Query.Model;
using Osta.Core.Feature.Service.Query.Result;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.SharedKernel.Logging;
using System.Diagnostics;

namespace Osta.Core.Feature.Service.Query.Handler
{
    public class ServiceQueryHandler : ResponseHandler,
        IRequestHandler<GetAllServicesQuery, Response<List<GetAllServiceResult>>>,
        IRequestHandler<GetServiceByIdQuery, Response<GetServiceByIdResult>>

    {
        private readonly IMapper mapper;
        private readonly IServiceService serviceService;
        private readonly ILoggerService loggerService;

        public ServiceQueryHandler(IMapper mapper, IServiceService serviceService, ILoggerService loggerService)
        {
            this.mapper = mapper;
            this.serviceService = serviceService;
            this.loggerService = loggerService;
        }
        public async Task<Response<List<GetAllServiceResult>>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            var Services = await serviceService.GetAllServicesAsync();

            sw.Stop();
            loggerService.LogInformation(
                "Handler took {Elapsed}  ms",
                sw.ElapsedMilliseconds);
            var result = mapper.Map<List<GetAllServiceResult>>(Services);
            loggerService.LogInformation(" All Services retrieved successfully  ");
            return Success(result, "Services retrieved successfully.");

        }

        public async Task<Response<GetServiceByIdResult>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var Service = await serviceService.GetServiceAsync(request.Id, cancellationToken);
                if (Service == null)
                {
                    loggerService.LogWarning("Attempted to retrieve non-existent Service {ServiceId}", request.Id);

                    return NotFound<GetServiceByIdResult>("Service not found.");
                }
                sw.Stop();
                loggerService.LogInformation(
                    "Handler took {Elapsed}  ms",
                    sw.ElapsedMilliseconds);
                var result = mapper.Map<GetServiceByIdResult>(Service);
                return Success(result, "Service retrieved successfully.");
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "An error occurred while retrieving the category with ID {CategoryId}.", request.Id);
                return BadRequest<GetServiceByIdResult>("An error occurred while processing your request.");

            }
        }
    }
}

