using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Data.Entities.Identity;
using Osta.Domain.Entities.Technician;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianCommandHandler
{
    public class VerifyTechnicianCommandHandler : ResponseHandler
, IRequestHandler<VerifyTechnicianCommand, Response<string>>
    {

        private readonly ITechnicianService technicianService;
        private readonly ILoggerService loggerService;

        private readonly ISendNotificationMessage sendNotificationMessage;
        private readonly UserManager<User> userManager;

        private readonly ITechnicianWalletService technicianWalletService;

        public VerifyTechnicianCommandHandler(ITechnicianService technicianService, ILoggerService loggerService,
             ISendNotificationMessage sendNotificationMessage, UserManager<User> userManager
          , ITechnicianWalletService technicianWalletService)
        {

            this.technicianService = technicianService;
            this.loggerService = loggerService;
            this.sendNotificationMessage = sendNotificationMessage;
            this.userManager = userManager;
            this.technicianWalletService = technicianWalletService;
        }

        public async Task<Response<string>> Handle(
        VerifyTechnicianCommand request,
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
                    "Verifying technician with ID {TechnicianId}",
                    request.TechId);

                await technicianService.VerifyRequestOfTechnicianAsync(request.TechId);

                var user = await userManager.FindByIdAsync(request.TechId);
                var notification = new TechnicianStatusNotificationMessage
                {
                    Id = technician.Id,
                    Email = user.Email,
                    ReasonOfReject = null,
                    StatusOfRequest = "Approved",
                    Message =
                     "Congratulations! Your technician request has been approved successfully. " +
                  "You can now start providing your services on Osta."
                };

                await sendNotificationMessage.SendNotification(
                    notification,
                    "technician-request");


                loggerService.LogInformation(
                    "Technician with ID {TechnicianId} verified successfully",
                    request.TechId);

                // Check If This Tech Has Wallet
                var TechHasWallet = await technicianWalletService.GetWalletAsync(request.TechId);
                if (TechHasWallet == null)
                {
                    // Create Wallet FOR THIS Technician

                    var wallet = new TechnicianWallet()
                    {
                        Amount = 0,
                        UpdatedAt = DateTime.UtcNow,
                        TechnicianId = request.TechId
                    };

                    var WalletResult = await technicianWalletService.CreateWalletAsync(wallet);
                    loggerService.LogInformation("Wallet Created Successfully");
                }

                var SetHimInRole = await userManager.AddToRoleAsync(user, "Technicians");
                return Success<string>($"Technician verified successfully");
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
                    "Error occurred while verifying technician with ID {TechnicianId}",
                    request.TechId);

                return BadRequest<string>("An error occurred while processing your request.");
            }
        }

    }
}
