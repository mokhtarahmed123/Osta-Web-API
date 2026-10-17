using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authentication.Query.Model
{
    public record MyProfileQuery() : IRequest<Response<MyProfileQueryResult>>
;
    public record MyProfileQueryResult(
        string Id,
        string FullName,
                string Email,
        string PhoneNumber
    );
}
