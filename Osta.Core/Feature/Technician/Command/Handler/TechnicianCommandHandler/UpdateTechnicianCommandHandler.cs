using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.Service.Model;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianCommandHandler
{
    public class UpdateTechnicianCommandHandler : ResponseHandler, IRequestHandler<UpdateTechnicianCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly ITechnicianService technicianService;
        private readonly ILoggerService loggerService;
        private readonly ITechnicianServiceAreasService technicianServiceAreasService;
        private readonly ITechnicianImagesService technicianImagesService;
        private readonly ICurrentUserService currentUser;

        public UpdateTechnicianCommandHandler(IMapper mapper, ITechnicianService technicianService, ILoggerService loggerService, ITechnicianServiceAreasService technicianServiceAreasService, ITechnicianImagesService technicianImagesService, ICurrentUserService currentUser)
        {
            this.mapper = mapper;
            this.technicianService = technicianService;
            this.loggerService = loggerService;
            this.technicianServiceAreasService = technicianServiceAreasService;
            this.technicianImagesService = technicianImagesService;
            this.currentUser = currentUser;
        }

        public async Task<Response<string>> Handle(
         UpdateTechnicianCommand request,
         CancellationToken cancellationToken)
        {
            var TechnicianId = currentUser.UserId;
            try
            {
                loggerService.LogInformation(
                    "Updating technician with ID {TechnicianId}",
                   TechnicianId);

                var technician = await technicianService.GetTechnicianAsync(
                    TechnicianId,
                    cancellationToken);

                if (technician is null)
                {
                    loggerService.LogWarning(
                        "Technician with ID {TechnicianId} was not found",
                        TechnicianId);

                    return NotFound<string>("Technician not found.");
                }

                mapper.Map(request, technician);

                await technicianService.UpdateTechnicianAsync(
                    TechnicianId,
                    technician,
                    cancellationToken);

                var Images = new TechnicianImageModel
                {
                    BackNationalIdImage = request.Images?.BackNationalIdImage,
                    FrontNationalIdImage = request.Images?.FrontNationalIdImage,
                    ProfileImage = request.Images?.ProfileImage
                };


                if (request.Images is not null)
                {
                    await technicianImagesService.Update(TechnicianId, Images);
                }

                var list = await technicianServiceAreasService.GetTechnicianServiceAreasByTechnicianIdAsync(TechnicianId, cancellationToken);
                if (list.Any())
                {
                    await technicianServiceAreasService.DeleteAllTechnicianServiceAreasWithSpecifyTechnicianIdAsync(
                        TechnicianId, cancellationToken);
                }

                if (request.ServiceAreas is { Count: > 0 })
                {
                    var technicianServiceAreas = request.ServiceAreas
                        .Select(serviceAreaId => new Osta.Data.Entities.Technician.TechnicianServiceArea
                        {
                            TechnicianId = TechnicianId,
                            ServiceAreaId = serviceAreaId
                        })
                        .ToList();

                    await technicianServiceAreasService
                        .AddTechnicianServiceAreasRangeAsync(
                            technicianServiceAreas, cancellationToken);
                }

                loggerService.LogInformation(
                    "Technician with ID {TechnicianId} updated successfully",
                            TechnicianId);

                return Updated<string>("Technician updated successfully.");
            }
            catch (Exception ex)
            {
                loggerService.LogError(
                    ex,
                    "Error occurred while updating technician with ID {TechnicianId}",
                    TechnicianId);

                return BadRequest<string>("An error occurred while processing your request.");
            }
        }


    }
}
