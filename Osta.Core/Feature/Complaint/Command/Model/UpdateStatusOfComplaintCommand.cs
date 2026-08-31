using MediatR;
using Osta.Core.Bases;
using Osta.Data.Enum;

namespace Osta.Core.Feature.Complaint.Command.Model
{
    public record UpdateStatusOfComplaintCommand(int Id, ComplaintStatus ComplaintStatus) : IRequest<Response<string>>
;
}
