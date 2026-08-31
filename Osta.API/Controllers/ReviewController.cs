using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Review.Command.Model;
using Osta.Core.Feature.Review.Query.Model;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : AppBaseController
    {
        [HttpPost]
        public async Task<IActionResult> Add(
            [FromBody] AddReviewCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }

        [HttpPut("{id:int}")]
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


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteReviewCommand(id);

            var response = await Mediator.Send(command);

            return NewResult(response);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetReviewByIdQuery(id);

            var response = await Mediator.Send(query);

            return NewResult(response);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyReviews()
        {
            var query = new GetAllMyReviewsAsUserQuery();

            var response = await Mediator.Send(query);

            return NewResult(response);
        }


        [HttpGet("technician/my")]
        public async Task<IActionResult> GetMyTechnicianReviews()
        {
            var query = new GetAllMyReviewsAsTechnicianQuery();

            var response = await Mediator.Send(query);

            return NewResult(response);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllReviewsQuery();

            var response = await Mediator.Send(query);

            return NewResult(response);
        }
    }
}
