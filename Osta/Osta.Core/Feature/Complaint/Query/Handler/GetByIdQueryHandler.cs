using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Complaint.Query.Model;
using Osta.Core.Feature.Complaint.Query.Result;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Complaint.Query.Handler
{
    public class GetByIdQueryHandler : ComplaintQueryHandler, IRequestHandler<
     GetByIdQuery,
     Response<GetByIdResult>>
    {
        public GetByIdQueryHandler(IComplaintService complaintService, IMapper mapper, ICurrentUserService currentUserService) : base(complaintService, mapper, currentUserService)
        {
        }

        public async Task<Response<GetByIdResult>>
           Handle(
               GetByIdQuery request,
               CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                return BadRequest<GetByIdResult>(
                    "Complaint ID must be greater than 0.");

            var complaint =
                await complaintService.GetById(
                    request.Id,
                    cancellationToken);

            if (complaint is null)
                return NotFound<GetByIdResult>(
                    "Complaint not found.");

            var result =
                mapper.Map<GetByIdResult>(
                    complaint);

            return Success(result);
        }
    }
}
