using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianPayout;
using Osta.Data.Entities.Identity;
using Osta.Notification.DTOs;
using Osta.Notification.Queue;
using Osta.Service.Abstract.TechnicianAbstract;

namespace Osta.Core.Feature.Technician.Command.Handler.TechnicianPayoutCommandHandler
{
    public class CompletePayoutCommandHandler
        : ResponseHandler,
          IRequestHandler<CompletePayoutCommand, Response<string>>
    {
        private readonly ITechnicianPayoutService _technicianPayoutService;
        private readonly ISendNotificationMessage _notificationService;
        private readonly UserManager<User> userManager;

        public CompletePayoutCommandHandler(
            ITechnicianPayoutService technicianPayoutService, ISendNotificationMessage sendNotificationMessage, UserManager<User> userManager)
        {
            _technicianPayoutService = technicianPayoutService;
            this._notificationService = sendNotificationMessage;
            this.userManager = userManager;
        }

        public async Task<Response<string>> Handle(
            CompletePayoutCommand request,
            CancellationToken cancellationToken)
        {
            var payout = await _technicianPayoutService.GetPayoutByIdAsync(request.Payout, cancellationToken);

            var result = await _technicianPayoutService
                .CompletePayoutAsync(
                    request.Payout,
                    cancellationToken);

            var User = await userManager.FindByIdAsync(payout.TechnicianId);
            if (!result)
                return NotFound<string>("Payout not found.");
            var notification = new PayoutNotification
            {
                TechnicianId = payout.TechnicianId,
                PayoutId = payout.Id,
                Amount = payout.Amount,
                ReceivingDetails = payout.ReceivingDetails,
                CompletedAt = DateTime.UtcNow,
                Method = payout.Method.ToString(),
                ReasonOfRejection = null,
                Email = User.Email,
                Message = $"تم تحويل مبلغ {payout.Amount} جنيه إلى {payout.ReceivingDetails} بنجاح."


            };

            await _notificationService.SendNotification(notification, queue: "payout-notification");
            return Success(
                "Payout completed successfully.");
        }
    }

}
