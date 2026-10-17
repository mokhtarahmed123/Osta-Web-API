using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Roles.Command.Model;
using Osta.Core.Feature.Roles.Query.Model;
using Osta.Core.Feature.Roles.Query.Result;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : AppBaseController
    {
        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new role", Description = "Allows an administrator to create a new role.")]
        [SwaggerResponse(201, "Role added successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid role data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Updates a role", Description = "Allows an administrator to update an existing role.")]
        [SwaggerResponse(200, "Role updated successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid role data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Role not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete("{roleId}")]
        [SwaggerOperation(Summary = "Deletes a role", Description = "Allows an administrator to delete an existing role.")]
        [SwaggerResponse(200, "Role deleted successfully", type: typeof(string))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Role not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var response = await Mediator.Send(new DeleteRoleCommand(roleId));


            return NewResult(response);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Gets all roles", Description = "Retrieves all roles available in the system.")]
        [SwaggerResponse(200, "List of roles returned successfully", type: typeof(List<GetAllRolesResult>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await Mediator.Send(new GetAllRolesQuery());
            return NewResult(response);
        }

        [HttpGet("{roleId}")]
        [SwaggerOperation(Summary = "Gets a role by ID", Description = "Retrieves a specific role using its unique identifier.")]
        [SwaggerResponse(200, "Role retrieved successfully", type: typeof(GetRoleByIdResult))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Role not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetRoleById([FromRoute] string roleId)
        {
            var response = await Mediator.Send(new GetRoleByIdQuery(roleId));
            return NewResult(response);
        }
    }
}
