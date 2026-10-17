using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianServiceCommandHandler
{
    public class TechnicianServiceCommandHandler : ResponseHandler, IRequestHandler<TechnicianAddServiceCommand, Response<string>>

    {

        private readonly IMapper mapper;
        private readonly ITechnicianService technicianService;
        private readonly ILoggerService loggerService;
        private readonly ITechnicianServiceService technicianServiceService;
        private readonly ITechnicianServiceAreasService technicianServiceAreasService;
        private readonly ICurrentUserService currentUser;

        public TechnicianServiceCommandHandler(IMapper mapper, ITechnicianService technicianService, ILoggerService loggerService, ITechnicianServiceService technicianServiceService, ITechnicianServiceAreasService technicianServiceAreasService, ICurrentUserService currentUser)
        {
            this.mapper = mapper;
            this.technicianService = technicianService;
            this.loggerService = loggerService;
            this.technicianServiceService = technicianServiceService;
            this.technicianServiceAreasService = technicianServiceAreasService;
            this.currentUser = currentUser;
        }
        public async Task<Response<string>> Handle(TechnicianAddServiceCommand request, CancellationToken cancellationToken)
        {
            var TechId = currentUser.UserId;
            try
            {
                var Technician = await technicianService.GetTechnicianAsync(TechId);
                if (Technician == null)
                {
                    loggerService.LogError($"Technician With Id {TechId} Not Found");
                    throw new KeyNotFoundException($"Technician With Id {TechId} Not Found");
                }

                var technicianServices = request.ServiceIds
               .Select(id => new TechnicianService
               {
                   TechnicianId = TechId,
                   ServiceId = id
               })
                .ToList();

                await technicianServiceService.AddRangeAsync(technicianServices, cancellationToken);
                loggerService.LogInformation($" Services For Technician With Id {TechId} Added  successfully");


                return Created($" Services For Technician With Id {TechId} Added  successfully");

            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "Error occurred while adding Services For Technician With Id {request.TechnicianId}");
                return BadRequest<string>("An error occurred while processing your request.");

            }


        }

    }
}
