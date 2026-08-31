using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Complaint.Command.Model
{
    public record AddComplaintCommand : IRequest<Response<string>>
    {
        public int BookingId { get; set; }
        public string Description { get; set; }


    }
}
