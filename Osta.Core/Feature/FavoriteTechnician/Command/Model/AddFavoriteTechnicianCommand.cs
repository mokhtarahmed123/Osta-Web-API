using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.FavoriteTechnician.Command.Model
{
    public record AddFavoriteTechnicianCommand(string TechnicianId) : IRequest<Response<string>>
;
}
