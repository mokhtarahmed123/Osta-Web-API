using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Complaint.Command.Model;
using Osta.Data.Enum;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Complaint.Command.Handler
{
    public class UpdateComplaintCommandHandler : ResponseHandler, IRequestHandler<UpdateComplaintCommand, Response<string>>
    {

        private readonly ICurrentUserService currentUserService;
        private readonly IComplaintService complaintService;


        public UpdateComplaintCommandHandler(ICurrentUserService currentUserService, IComplaintService complaintService)
        {

            this.currentUserService = currentUserService;
            this.complaintService = complaintService;

        }

        public async Task<Response<string>> Handle(
      UpdateComplaintCommand request,
      CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.Id <= 0)
                return BadRequest<string>(
                    "Complaint ID must be greater than 0.");

            var customerId = currentUserService.UserId;

            if (string.IsNullOrEmpty(customerId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            // Get complaint
            var complaint =
                await complaintService.GetById(
                    request.Id,
                    cancellationToken);

            if (complaint is null)
                return NotFound<string>(
                    "Complaint not found.");

            // Check ownership
            if (complaint.Booking.CustomerId != customerId)
                return Unauthorized<string>(
                    "You cannot update this complaint.");

            // Don't allow updating resolved complaint
            if (complaint.Status == ComplaintStatus.Resolved)
                return BadRequest<string>(
                    "You cannot update a resolved complaint.");

            // Only update Description
            complaint.Description =
                request.Description;

            await complaintService.Update(
                request.Id,
                complaint,
                cancellationToken);

            return Success<string>(
                "Complaint updated successfully.");
        }
    }
}
