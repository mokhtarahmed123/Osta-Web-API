using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Roles.Command.Model;
using Osta.Core.Feature.Roles.Query.Model;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : AppBaseController
    {
        [HttpPost]

        public async Task<IActionResult> AddRole([FromBody] AddRoleCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var response = await Mediator.Send(new DeleteRoleCommand(roleId));


            return NewResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await Mediator.Send(new GetAllRolesQuery());
            return NewResult(response);
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById([FromRoute] string roleId)
        {
            var response = await Mediator.Send(new GetRoleByIdQuery(roleId));
            return NewResult(response);
        }
    }
}
