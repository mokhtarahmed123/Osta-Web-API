using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Review.Command.Model;
using Osta.Core.Feature.Review.Query.Model;
using Osta.Core.Feature.Review.Query.Result;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : AppBaseController
    {
        [Authorize(Roles = "User")]

        [HttpPost]

        [SwaggerOperation(Summary = "Creates a new review", Description = "Allows an authenticated user to create a review for a completed booking.")]
        [SwaggerResponse(201, "Review added successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid review data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Booking or technician not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Add(
            [FromBody] AddReviewCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [Authorize(Roles = "User")]
        [HttpPut("{id:int}")]

        [SwaggerOperation(Summary = "Updates a review", Description = "Updates an existing review using its unique identifier.")]
        [SwaggerResponse(200, "Review updated successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid review data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Review not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateReviewCommand command)
        {
            command = command with
            {
                Id = id
            };

            var response = await Mediator.Send(command);

            return NewResult(response);
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{id:int}")]

        [SwaggerOperation(Summary = "Deletes a review", Description = "Deletes an existing review using its unique identifier.")]
        [SwaggerResponse(200, "Review deleted successfully", type: typeof(string))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Review not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteReviewCommand(id);

            var response = await Mediator.Send(command);

            return NewResult(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Gets a review by ID", Description = "Retrieves a specific review using its unique identifier.")]
        [SwaggerResponse(200, "Review retrieved successfully", type: typeof(GetReviewByIdResult))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Review not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetReviewByIdQuery(id);

            var response = await Mediator.Send(query);

            return NewResult(response);
        }
        [Authorize(Roles = "User")]
        [HttpGet("my")]

        [SwaggerOperation(Summary = "Gets my reviews", Description = "Retrieves all reviews created by the authenticated user.")]
        [SwaggerResponse(200, "User reviews returned successfully", type: typeof(List<GetAllMyReviewsAsUserResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetMyReviews()
        {
            var query = new GetAllMyReviewsAsUserQuery();

            var response = await Mediator.Send(query);

            return NewResult(response);
        }

        [Authorize(Roles = "Technicians")]
        [HttpGet("technician/my")]

        [SwaggerOperation(Summary = "Gets my technician reviews", Description = "Retrieves all reviews associated with the authenticated technician.")]
        [SwaggerResponse(200, "Technician reviews returned successfully", type: typeof(List<GetAllMyReviewsAsTechnicianResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetMyTechnicianReviews()
        {
            var query = new GetAllMyReviewsAsTechnicianQuery();

            var response = await Mediator.Send(query);

            return NewResult(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [SwaggerOperation(Summary = "Gets all reviews", Description = "Retrieves all reviews available in the system.")]
        [SwaggerResponse(200, "List of reviews returned successfully", type: typeof(List<GetAllReviewsResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllReviewsQuery();

            var response = await Mediator.Send(query);

            return NewResult(response);
        }
    }
}
