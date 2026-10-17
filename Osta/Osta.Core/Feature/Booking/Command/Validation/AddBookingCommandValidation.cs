
using FluentValidation;
using Osta.Core.Feature.Booking.Command.Model.CustomerModel;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.Service.Abstract.TechnicianAbstract;

namespace Osta.Core.Feature.Booking.Command.Validation
{
    public class AddBookingCommandValidation
        : AbstractValidator<AddBookingCommand>
    {
        private readonly ITechnicianService technicianService;
        private readonly ITechnicianServiceAreasService technicianServiceAreasService;
        private readonly IServiceAreaService serviceAreaService;
        private readonly IServiceService serviceService;

        public AddBookingCommandValidation(ITechnicianService technicianService, ITechnicianServiceAreasService technicianServiceAreasService, IServiceAreaService serviceAreaService, IServiceService serviceService)
        {

            this.technicianService = technicianService;
            this.technicianServiceAreasService = technicianServiceAreasService;
            this.serviceAreaService = serviceAreaService;
            this.serviceService = serviceService;
            Validate();
        }
        private void Validate()
        {
            RuleFor(x => x.TechnicianId)
                       .NotEmpty()
                       .WithMessage("Technician ID is required.")
                       .MustAsync(TechnicianExists)
                       .WithMessage("Technician does not exist.");
            RuleFor(x => x.ServiceId).NotEmpty().WithMessage("Service ID is required.").MustAsync(ServiceExists).WithMessage("Service does not exist.");

            RuleFor(x => x.Area)
                .NotEmpty()
                .WithMessage("Area is required.");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required.");

            RuleFor(x => x.Governorate)
                .NotEmpty()
                .WithMessage("Governorate is required.");

            RuleFor(x => x.Street)
                .NotEmpty()
                .WithMessage("Street is required.");

            RuleFor(x => x)
                .MustAsync(TechnicianIsInSameCity)
                .WithMessage("The selected technician is not available in this city.");




        }
        private async Task<bool> TechnicianExists(
            string technicianId,
            CancellationToken cancellationToken)
        {
            var technician =
                await technicianService.GetTechnicianAsync(technicianId);

            return technician is not null;
        }

        private async Task<bool> ServiceExists(int serviceId, CancellationToken cancellationToken) { var service = await serviceService.GetServiceAsync(serviceId); return service is not null; }


        private async Task<bool> TechnicianIsInSameCity(
            AddBookingCommand command,
            CancellationToken cancellationToken)
        {
            var ServiceArea = await serviceAreaService.GetServiceByCity(command.City);
            if (ServiceArea is null)
                return false;
            var technician =
                await technicianService.GetTechnicianAsync(command.TechnicianId);

            if (technician is null)
                return false;

            var Result = await technicianServiceAreasService.
                TechnicianHasThisServiceAreaAsync(technician.Id, ServiceArea.Id, cancellationToken);

            return Result;
        }


    }
}

