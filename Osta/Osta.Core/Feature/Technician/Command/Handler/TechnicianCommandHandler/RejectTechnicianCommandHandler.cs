using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Data.Entities.Identity;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianCommandHandler
{
    public class RejectTechnicianCommandHandler : ResponseHandler, IRequestHandler<RejectTechnicianCommand, Response<string>>
    {
        private readonly ITechnicianService technicianService;
        private readonly ILoggerService loggerService;

        private readonly UserManager<User> userManager;
        private readonly ISendNotificationMessage sendNotificationMessage;


        public RejectTechnicianCommandHandler(ITechnicianService technicianService, ILoggerService loggerService, UserManager<User> userManager, ISendNotificationMessage sendNotificationMessage)
        {

            this.technicianService = technicianService;
            this.loggerService = loggerService;

            this.userManager = userManager;
            this.sendNotificationMessage = sendNotificationMessage;

        }

        public async Task<Response<string>> Handle(
     RejectTechnicianCommand request,
     CancellationToken cancellationToken)
        {
            try
            {
                var technician = await technicianService.GetTechnicianAsync(
           request.TechId,
            cancellationToken);

                if (technician is null)
                    return NotFound<string>("Technician not found.");

                loggerService.LogInformation(
                    "Rejecting technician with ID {TechnicianId}",
                    request.TechId);

                await technicianService.RejectRequestOfTechnicianAsync(
                    request.TechId,
                    request.Reason);
                var user = await userManager.FindByIdAsync(request.TechId);
                var notification = new TechnicianStatusNotificationMessage
                {
                    Id = technician.Id,
                    Email = user.Email,
                    ReasonOfReject = request.Reason,
                    StatusOfRequest = "Rejected",
                    Message =
                "Unfortunately, your technician request has been rejected. " +
                "Please review the provided information and try again."
                };

                await sendNotificationMessage.SendNotification(
                    notification,
                    "technician-request");
                loggerService.LogInformation(
                    "Technician with ID {TechnicianId} rejected successfully",
                    request.TechId);

                return Success<string>("Technician rejected successfully.");
            }
            catch (KeyNotFoundException)
            {
                loggerService.LogWarning(
                    "Technician with ID {TechnicianId} was not found",
                    request.TechId);

                return NotFound<string>("Technician not found.");
            }
            catch (Exception ex)
            {
                loggerService.LogError(
                    ex,
                    "Error occurred while rejecting technician with ID {TechnicianId}",
                    request.TechId);

                return BadRequest<string>("An error occurred while processing your request.");
            }
        }

    }
}
