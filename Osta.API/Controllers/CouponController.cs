using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Core.Feature.Coupon.Command.Result;
using Osta.Core.Feature.Coupon.Query.Model;
using Osta.Core.Feature.Coupon.Query.Result;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CouponController : AppBaseController
    {
        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new coupon", Description = " Admin Creates a new coupon and stores it in the database.")]
        [SwaggerResponse(201, "Coupon added successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid coupon data")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Add([FromBody] AddCouponCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("bulk")]
        [SwaggerOperation(Summary = "Creates multiple coupons", Description = " Admin Creates multiple coupons in a single request.")]
        [SwaggerResponse(201, "Coupons added successfully", type: typeof(List<string>))]
        [SwaggerResponse(400, "Invalid coupon data")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> AddRange([FromBody] AddRangeCouponsCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]

        [SwaggerOperation(Summary = "Updates a coupon", Description = "  Admin Updates an existing coupon using its unique identifier.")]
        [SwaggerResponse(200, "Coupon updated successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid coupon data or route ID does not match body ID")]
        [SwaggerResponse(404, "Coupon not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCouponCommand command)
        {
            if (id != command.Id)
                return BadRequest("Route Id and body Id do not match.");

            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]

        [SwaggerOperation(Summary = "Deletes a coupon", Description = "Admin Deletes an existing coupon using its unique identifier.")]
        [SwaggerResponse(200, "Coupon deleted successfully", type: typeof(string))]
        [SwaggerResponse(404, "Coupon not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Mediator.Send(new DeleteCouponCommand(id));
            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Gets a coupon by ID", Description = " Admin Retrieves a specific coupon using its unique identifier.")]
        [SwaggerResponse(200, "Coupon retrieved successfully", type: typeof(GetCouponByIdResult))]
        [SwaggerResponse(404, "Coupon not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetCouponByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("code/{code}")]

        [SwaggerOperation(Summary = "Gets a coupon by code", Description = " Admin Retrieves a coupon using its unique coupon code.")]
        [SwaggerResponse(200, "Coupon retrieved successfully", type: typeof(GetCouponByCodeResult))]
        [SwaggerResponse(404, "Coupon not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await Mediator.Send(new GetCouponByCodeQuery(code));
            return Ok(result);
        }

        [HttpGet]

        [SwaggerOperation(Summary = "Gets all coupons", Description = " Admin Retrieves all coupons based on their active status.")]
        [SwaggerResponse(200, "List of coupons returned successfully", type: typeof(List<GetAllCouponResult>))]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAll([FromQuery] bool isActive = true)
        {
            var result = await Mediator.Send(new GetAllCouponQuery(isActive));
            return Ok(result);
        }
        [HttpPost("apply")]
        [SwaggerOperation(Summary = "Applies a coupon", Description = " Admin Validates and applies a coupon to the current operation.")]
        [SwaggerResponse(200, "Coupon applied successfully", type: typeof(ApplyCouponResult))]
        [SwaggerResponse(400, "Invalid or expired coupon")]
        [SwaggerResponse(404, "Coupon not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> Apply([FromBody] ApplyCouponCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}