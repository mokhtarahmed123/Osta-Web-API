using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Complaint.Query.Model;
using Osta.Core.Feature.Complaint.Query.Result;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Complaint.Query.Handler
{
    public class GetAllComplaintQueryHandler : ComplaintQueryHandler, IRequestHandler<
            GetAllComplaintQuery,
            Response<List<GetAllComplaintResult>>>
    {
        public GetAllComplaintQueryHandler(IComplaintService complaintService, IMapper mapper, ICurrentUserService currentUserService) : base(complaintService, mapper, currentUserService)
        {
        }

        public async Task<Response<List<GetAllComplaintResult>>>
         Handle(
             GetAllComplaintQuery request,
             CancellationToken cancellationToken)
        {
            var complaints =
                await complaintService.GetAllComplaints(
                    cancellationToken);

            var result =
                mapper.Map<List<GetAllComplaintResult>>(
                    complaints);

            return Success(result);
        }

    }
}
