using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Complaint.Command.Model;
using Osta.Data.Enum;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Complaint.Command.Handler
{
    public class UpdateStatusOfComplaintCommandHandler : ResponseHandler, IRequestHandler<UpdateStatusOfComplaintCommand, Response<string>>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IComplaintService complaintService;
        private readonly ILoggerService loggerService;

        public UpdateStatusOfComplaintCommandHandler(ICurrentUserService currentUserService, IComplaintService complaintService, ILoggerService loggerService)
        {
            this.currentUserService = currentUserService;
            this.complaintService = complaintService;
            this.loggerService = loggerService;
        }

        public async Task<Response<string>> Handle(
    UpdateStatusOfComplaintCommand request,
    CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.Id <= 0)
                return BadRequest<string>(
                    "Complaint ID must be greater than 0.");

            var complaint =
                await complaintService.GetById(
                    request.Id,
                    cancellationToken);

            if (complaint is null)
                return NotFound<string>(
                    "Complaint not found.");
            if (complaint.Status == ComplaintStatus.Resolved)
                return BadRequest<string>(
                    "Resolved complaint cannot be updated.");
            await complaintService.UpdateStatus(
                request.Id,
                request.ComplaintStatus,
                cancellationToken);

            loggerService.LogInformation(
                $"Complaint Id {request.Id} status changed " +
                $"from {complaint.Status} to {request.ComplaintStatus}.");

            return Success<string>(
                $"Complaint status updated successfully to {request.ComplaintStatus}.");
        }
    }
}
