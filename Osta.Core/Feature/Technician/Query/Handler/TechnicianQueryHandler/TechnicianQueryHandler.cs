using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Service.Query.Result;
using Osta.Core.Feature.ServiceArea.Query.Result;
using Osta.Core.Feature.Technician.Query.Model.TechnicianModel;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnician;
using Osta.Core.Wrappers;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Osta.Core.Feature.Technician.Query.Handler.TechnicianQueryHandler
{
    public class TechnicianQueryHandler : ResponseHandler,
        IRequestHandler<GetAllTechniciansQuery, Response<List<GetAllTechniciansResult>>>,
        IRequestHandler<GetAllTechniciansPaginatedQuery, PaginatedResult<GetAllTechniciansPaginatedResult>>,
        IRequestHandler<GetAllTechniciansWithRateQuery, Response<List<GetAllTechniciansWithRateResult>>>,
        IRequestHandler<GetAllTechniciansSearchQuery, Response<List<GetAllTechniciansSearchResult>>>,
        IRequestHandler<GetTechnicianByIdQuery, Response<GetTechnicianByIdResult>>

    {
        private readonly IMapper mapper;
        private readonly ILoggerService loggerService;
        private readonly ITechnicianService technicianService;
        private readonly ITechnicianServiceService technicianServiceService;
        private readonly ITechnicianServiceAreasService technicianServiceAreasService;
        private readonly IServiceService serviceService;
        private readonly IServiceAreaService serviceAreaService;

        public TechnicianQueryHandler(IMapper mapper, ILoggerService loggerService, ITechnicianService technicianService, ITechnicianServiceService technicianServiceService, ITechnicianServiceAreasService technicianServiceAreasService, IServiceService serviceService, IServiceAreaService serviceAreaService)
        {
            this.mapper = mapper;
            this.loggerService = loggerService;
            this.technicianService = technicianService;
            this.technicianServiceService = technicianServiceService;
            this.technicianServiceAreasService = technicianServiceAreasService;
            this.serviceService = serviceService;
            this.serviceAreaService = serviceAreaService;
        }
        public async Task<Response<List<GetAllTechniciansResult>>> Handle(GetAllTechniciansQuery request, CancellationToken cancellationToken)
        {
            var technicians = await technicianService.GetAllTechniciansAsync();
            var services = await technicianServiceService.GetAllAsync(cancellationToken);

            var serviceAreas = await technicianServiceAreasService.GetAllTechnicianServiceAreasAsync(cancellationToken);
            var result = mapper.Map<List<GetAllTechniciansResult>>(technicians);


            foreach (var technician in result)
            {
                technician.TechnicianService =
                    services.Where(x => x.TechnicianId == technician.Id).ToList();

                technician.TechnicianServiceArea =
                    serviceAreas.Where(x => x.TechnicianId == technician.Id).ToList();
            }

            return Success(result);
        }

        public async Task<PaginatedResult<GetAllTechniciansPaginatedResult>> Handle(
            GetAllTechniciansPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            var Sw = Stopwatch.StartNew();

            Expression<Func<Technicians, GetAllTechniciansPaginatedResult>> expression =
                e => new GetAllTechniciansPaginatedResult(
                    e.Id,
                    e.Bio,
                    e.IsVerified,
                    e.Rating,

                    e.TotalReviews,
                    e.CompletedBookings,
                    e.YearsOfExperience,
                    e.CreatedAt,
                    e.ReasonOfReject,
                    e.Status.ToString()
                );


            var query = technicianService
                .GetTechniciansQueryable().OrderBy(c => c.CreatedAt)
                .Select(expression);


            var paginatedList = await query.ToPaginatedListAsync(
                request.PageNumber,
                request.PageSize);

            Sw.Stop();

            loggerService.LogInformation(
                "Handler finished in {Elapsed} ms",
                Sw.ElapsedMilliseconds);

            return paginatedList;
        }

        public async Task<Response<List<GetAllTechniciansWithRateResult>>> Handle(GetAllTechniciansWithRateQuery request, CancellationToken cancellationToken)
        {
            loggerService.LogInformation(
       "Getting technicians with rating greater than or equal to {Rating}",
    request.Rate);
            var technicians = await technicianService.GetTechniciansByMinimumRateAsync(request.Rate);
            var result = mapper.Map<List<GetAllTechniciansWithRateResult>>(technicians);
            loggerService.LogInformation(
     "{Count} technicians found with rating greater than or equal to {Rating}",
     result.Count,
     request.Rate);

            return Success(result);

        }

        public Task<Response<List<GetAllTechniciansSearchResult>>> Handle(GetAllTechniciansSearchQuery request, CancellationToken cancellationToken)
        {
            var query = technicianService.GetTechniciansQueryable();

            if (request.IsVerified.HasValue)
                query = query.Where(x => x.IsVerified == request.IsVerified.Value);

            if (request.Status.HasValue)
                query = query.Where(x => x.Status == request.Status.Value);

            if (request.MinRating.HasValue)
                query = query.Where(x => x.Rating >= request.MinRating.Value);

            if (request.MinYearsOfExperience.HasValue)
                query = query.Where(x => x.YearsOfExperience >= request.MinYearsOfExperience.Value);

            var Result = mapper.Map<List<GetAllTechniciansSearchResult>>(query);

            return Task.FromResult(
                    Success<List<GetAllTechniciansSearchResult>>(Result));
        }

        public async Task<Response<GetTechnicianByIdResult>> Handle(GetTechnicianByIdQuery request, CancellationToken cancellationToken)
        {
            var technician = await technicianService.GetTechnicianWithServiceAndServiceAreaAsync(request.TechnicianId);
            if (technician is null)
            {
                loggerService.LogWarning(
                    "Technician with ID {TechnicianId} was not found",
                    request.TechnicianId);

                return NotFound<GetTechnicianByIdResult>("Technician not found.");
            }


            var result = mapper.Map<GetTechnicianByIdResult>(technician);

            result.Services = mapper.Map<List<GetServiceByIdResult>>(
              await serviceService.GetServicesByTechnicianIdAsync(request.TechnicianId));

            result.Areas = mapper.Map<List<GetAllServiceAreasResult>>(
          await serviceAreaService.GetServiceAreaWithSpecificTechIdAsync(request.TechnicianId));

            return Success(result);

        }
    }
}
