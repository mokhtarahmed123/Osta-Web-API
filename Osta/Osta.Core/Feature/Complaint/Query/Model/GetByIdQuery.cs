using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Complaint.Query.Result;

namespace Osta.Core.Feature.Complaint.Query.Model
{
    public record GetByIdQuery(int Id) : IRequest<Response<GetByIdResult>>;
}
