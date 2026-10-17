using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Complaint.Query.Model;
using Osta.Core.Feature.Complaint.Query.Result;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Complaint.Query.Handler
{
    public class GetMyComplaintsAsUserQueryHandler : ComplaintQueryHandler, IRequestHandler<
            GetMyComplaintsAsUserQuery,
            Response<List<GetMyComplaintsAsUserResult>>>
    {
        public GetMyComplaintsAsUserQueryHandler(IComplaintService complaintService, IMapper mapper, ICurrentUserService currentUserService) : base(complaintService, mapper, currentUserService)
        {
        }

        public async Task<Response<List<GetMyComplaintsAsUserResult>>>
            Handle(
                GetMyComplaintsAsUserQuery request,
                CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            var complaints =
                await complaintService.GetMyComplaints(
                    userId,
                    cancellationToken);

            var result =
                mapper.Map<List<GetMyComplaintsAsUserResult>>(
                    complaints);

            return Success(result);
        }
    }
}
