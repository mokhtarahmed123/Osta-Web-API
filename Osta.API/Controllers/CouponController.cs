using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Coupon.Command.Model;
using Osta.Core.Feature.Coupon.Query.Model;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CouponController : AppBaseController
    {
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddCouponCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> AddRange([FromBody] AddRangeCouponsCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCouponCommand command)
        {
            if (id != command.Id)
                return BadRequest("Route Id and body Id do not match.");

            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Mediator.Send(new DeleteCouponCommand(id));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetCouponByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await Mediator.Send(new GetCouponByCodeQuery(code));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool isActive = true)
        {
            var result = await Mediator.Send(new GetAllCouponQuery(isActive));
            return Ok(result);
        }
        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyCouponCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}