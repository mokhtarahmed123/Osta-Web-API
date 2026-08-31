using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Core.Feature.Technician.Query.Model.TechnicianModel;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnician;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]

    public class TechnicianController : AppBaseController
    {
        [Authorize]
        [HttpPost("Request")]
        [Consumes("multipart/form-data")]

        [SwaggerOperation(Summary = "Creates a technician request", Description = "Allows an authenticated user to submit a request to become a technician.")]
        [SwaggerResponse(201, "Technician request created successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid technician data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> AddTechnician(
            [FromForm] AddTechnicianCommand addTechnicianCommand,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(addTechnicianCommand, cancellationToken);
            return NewResult(response);
        }


        [HttpGet("{Id}")]

        [SwaggerOperation(Summary = "Gets a technician by ID", Description = "Retrieves a specific technician using their unique identifier.")]
        [SwaggerResponse(200, "Technician retrieved successfully", type: typeof(GetTechnicianByIdResult))]
        [SwaggerResponse(404, "Technician not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]

        public async Task<IActionResult> GetById(string Id)
        {
            var Query = new GetTechnicianByIdQuery(Id);
            var response = await Mediator.Send(Query);
            return NewResult(response);
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Gets all technicians", Description = "Retrieves all technicians available in the system.")]
        [SwaggerResponse(200, "List of technicians returned successfully", type: typeof(List<GetAllTechniciansResult>))]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAll()
        {
            var Query = new GetAllTechniciansQuery();
            var response = await Mediator.Send(Query);
            return NewResult(response);
        }


        [HttpGet("Paginated/{pageNumber:int}/{pageSize:int}")]

        [HttpGet("Paginated/{pageNumber:int}/{pageSize:int}")]
        [SwaggerOperation(Summary = "Gets paginated technicians", Description = "Retrieves technicians using pagination.")]
        [SwaggerResponse(200, "Paginated technicians returned successfully", type: typeof(GetAllTechniciansPaginatedResult))]
        [SwaggerResponse(400, "Invalid pagination parameters")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Paginated(int pageNumber, int pageSize)
        {
            var query = new GetAllTechniciansPaginatedQuery(pageNumber, pageSize);

            var response = await Mediator.Send(query);

            return Ok(response);
        }
        [Authorize]

        [HttpGet("rate/{rate}")]

        [SwaggerOperation(Summary = "Gets technicians by rating", Description = "Retrieves technicians whose rating matches the specified rate.")]
        [SwaggerResponse(200, "Technicians returned successfully", type: typeof(List<GetAllTechniciansWithRateResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "No technicians found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAllTechniciansWithRate(double rate)
        {
            var Query = new GetAllTechniciansWithRateQuery(rate);
            var response = await Mediator.Send(Query);
            return NewResult(response);


        }

        [HttpGet("Search")]

        [SwaggerOperation(Summary = "Searches for technicians", Description = "Searches and filters technicians based on the provided criteria.")]
        [SwaggerResponse(200, "Technicians returned successfully", type: typeof(List<GetAllTechniciansSearchResult>))]
        [SwaggerResponse(400, "Invalid search parameters")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Search([FromQuery] GetAllTechniciansSearchQuery query)
        {
            var response = await Mediator.Send(query);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}")]

        [SwaggerOperation(Summary = "Deletes a technician", Description = "Allows an administrator to delete a technician.")]
        [SwaggerResponse(200, "Technician deleted successfully", type: typeof(string))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Technician not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Delete(string Id)
        {
            {
                var command = new DeleteTechnicianCommand(Id);
                var response = await Mediator.Send(command);
                return NewResult(response);

            }

        }
        [Authorize(Roles = "Technicians")]
        [Consumes("multipart/form-data")]
        [HttpPatch]

        [SwaggerOperation(Summary = "Updates technician profile", Description = "Allows an authenticated technician to update their profile information.")]
        [SwaggerResponse(200, "Technician updated successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid technician data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Technician not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Update(

    [FromForm] UpdateTechnicianCommand command)
        {

            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [Authorize(Roles = "Technicians")]
        [HttpGet("My-Profile")]
        [SwaggerOperation(Summary = "Gets my technician profile", Description = "Retrieves the profile of the currently authenticated technician.")]
        [SwaggerResponse(200, "Technician profile returned successfully", type: typeof(GetMyProfileResult))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Technician profile not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> MyProfile()
        {
            var response = await Mediator.Send(new GetMyProfileQuery());
            return NewResult(response);


        }






    }
}
