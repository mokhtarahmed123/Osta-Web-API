using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Data.Entities.Technician;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.Service.Model;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianCommandHandler
{
    public class RequestTechnicianCommandHandler : ResponseHandler, IRequestHandler<AddTechnicianCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly ITechnicianService technicianService;
        private readonly ILoggerService loggerService;

        private readonly ITechnicianServiceAreasService technicianServiceAreasService;
        private readonly ITechnicianImagesService technicianImagesService;
        private readonly ICurrentUserService currentUser;

        public RequestTechnicianCommandHandler(IMapper mapper, ITechnicianService technicianService, ILoggerService loggerService, ITechnicianServiceAreasService technicianServiceAreasService, ITechnicianImagesService technicianImagesService, ICurrentUserService currentUser)
        {
            this.mapper = mapper;
            this.technicianService = technicianService;
            this.loggerService = loggerService;

            this.technicianServiceAreasService = technicianServiceAreasService;
            this.technicianImagesService = technicianImagesService;
            this.currentUser = currentUser;
        }

        public async Task<Response<string>> Handle(AddTechnicianCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = currentUser.UserId;
                loggerService.LogInformation("Adding new Technician");


                var existingTechnician = await technicianService.GetTechnicianAsync(userId, cancellationToken);
                if (existingTechnician is not null)
                {
                    loggerService.LogWarning("Technician with ID {TechnicianId} already exists", userId);
                    return BadRequest<string>("Technician already exists.");
                }

                var technician = mapper.Map<Technicians>(request);
                technician.Id = userId;

                await technicianService.AddTechnicianAsync(technician, cancellationToken);

                var TechnicianImage = new TechnicianImageModel
                {
                    BackNationalIdImage = request.Images.BackNationalIdImage,
                    FrontNationalIdImage = request.Images.FrontNationalIdImage,
                    ProfileImage = request.Images.ProfileImage

                };
                await technicianImagesService.Add(userId, TechnicianImage);

                loggerService.LogInformation("Technician created successfully");

                if (request.ServiceAreas is { Count: > 0 })
                {
                    var technicianServiceAreas = request.ServiceAreas
                        .Select(serviceAreaId => new Osta.Data.Entities.Technician.TechnicianServiceArea
                        {
                            ServiceAreaId = serviceAreaId,
                            TechnicianId = technician.Id
                        })
                        .ToList();

                    await technicianServiceAreasService.AddTechnicianServiceAreasRangeAsync(technicianServiceAreas, cancellationToken);
                }

                loggerService.LogInformation("Technician service areas added successfully");

                return Created<string>("Technician created successfully.");
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "Error occurred while adding Technician");
                return BadRequest<string>("An error occurred while processing your request.");
            }
        }

    }
}
