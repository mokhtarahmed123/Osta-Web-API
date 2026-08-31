using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.FavoriteTechnician.Command.Model;
using Osta.Core.Feature.FavoriteTechnician.Query.Model;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class FavoriteTechnicianController : AppBaseController
    {
        [HttpGet("my")]
        public async Task<IActionResult> GetMyFavorites()
        {
            var query = new GetMyFavoriteQuery();

            var response = await Mediator.Send(query);

            return NewResult(response);
        }
        [HttpPost("{technicianId}")]
        public async Task<IActionResult> Add(
    string technicianId)
        {
            var command =
                new AddFavoriteTechnicianCommand(technicianId);

            var response =
                await Mediator.Send(command);

            return NewResult(response);
        }

        [HttpDelete("{technicianId}")]
        public async Task<IActionResult> Delete(
            string technicianId)
        {
            var command =
                new DeleteFavoriteTechnicianCommand(
                    technicianId);

            var response =
                await Mediator.Send(command);

            return NewResult(response);
        }

    }
}
