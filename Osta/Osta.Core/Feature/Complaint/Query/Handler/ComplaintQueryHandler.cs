using AutoMapper;
using Osta.Core.Abstract.AdministrationAbstract;
using Osta.Core.Bases;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Complaint.Query.Handler
{
    public class ComplaintQueryHandler :
        ResponseHandler



    {
        protected readonly IComplaintService complaintService;
        protected readonly IMapper mapper;
        protected readonly ICurrentUserService currentUserService;

        public ComplaintQueryHandler(
            IComplaintService complaintService,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            this.complaintService = complaintService;
            this.mapper = mapper;
            this.currentUserService = currentUserService;
        }





    }
}