using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.FavoriteTechnician.Command.Model;
using Osta.Core.Feature.FavoriteTechnician.Query.Model;
using Osta.Core.Feature.FavoriteTechnician.Query.Result;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class FavoriteTechnicianController : AppBaseController
    {
        [HttpGet("my")]
        [SwaggerOperation(Summary = "Gets my favorite technicians", Description = "Retrieves all technicians marked as favorites by the authenticated user.")]
        [SwaggerResponse(200, "Favorite technicians returned successfully", type: typeof(List<GetMyFavoriteResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetMyFavorites()
        {
            var query = new GetMyFavoriteQuery();

            var response = await Mediator.Send(query);

            return NewResult(response);
        }
        [HttpPost("{technicianId}")]
        [SwaggerOperation(Summary = "Adds a technician to favorites", Description = "Adds a technician to the authenticated user's favorite technicians.")]
        [SwaggerResponse(200, "Technician added to favorites successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid technician ID")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Technician not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
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
        [SwaggerOperation(Summary = "Removes a technician from favorites", Description = "Removes a technician from the authenticated user's favorite technicians.")]
        [SwaggerResponse(200, "Technician removed from favorites successfully", type: typeof(string))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Favorite technician not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
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
