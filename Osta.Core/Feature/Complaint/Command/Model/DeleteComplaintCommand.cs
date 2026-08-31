using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Complaint.Command.Model
{
    public record DeleteComplaintCommand(int Id) : IRequest<Response<string>>
   ;

}
