using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Complaint.Query.Model;
using Osta.Core.Feature.Complaint.Query.Result;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Complaint.Query.Handler
{
    public class GetByBookingIdQueryHandler : ComplaintQueryHandler,
        IRequestHandler<
            GetByBookingIdQuery,
            Response<List<GetByBookingIdResult>>>
    {
        public GetByBookingIdQueryHandler(IComplaintService complaintService, IMapper mapper, ICurrentUserService currentUserService) : base(complaintService, mapper, currentUserService)
        {
        }

        public async Task<Response<List<GetByBookingIdResult>>>
            Handle(
                GetByBookingIdQuery request,
                CancellationToken cancellationToken)
        {
            if (request.BookingId <= 0)
                return BadRequest<List<GetByBookingIdResult>>(
                    "Booking ID must be greater than 0.");

            var complaints =
                await complaintService.GetByBookingId(
                    request.BookingId,
                    cancellationToken);

            var result =
                mapper.Map<List<GetByBookingIdResult>>(
                    complaints);

            return Success(result);
        }

    }
}
