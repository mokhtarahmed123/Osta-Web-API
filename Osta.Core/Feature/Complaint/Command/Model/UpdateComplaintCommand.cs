using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Complaint.Command.Model
{
    public record UpdateComplaintCommand(int Id) : IRequest<Response<string>>
    {
        public string Description { get; set; }
    }
}
