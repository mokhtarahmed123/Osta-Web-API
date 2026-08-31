using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.FavoriteTechnician.Command.Model
{
    public record DeleteFavoriteTechnicianCommand(string TechnicianId) : IRequest<Response<string>>
;
}
