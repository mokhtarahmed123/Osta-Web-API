using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Authorization.Command.Model.PermissionModel;
using Osta.Core.Feature.Authorization.Command.Model.Roles;
using Osta.Core.Feature.Authorization.Query.Model;
using Osta.Core.Feature.Authorization.Query.Model.PermissionModel;
using Swashbuckle.AspNetCore.Annotations;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class AuthorizationController : AppBaseController
    {
        [HttpPost("AssignRole/{UserId}/{RoleId}")]
        [SwaggerOperation(Summary = "Assigns a role to a user", Description = "Allows an administrator to assign a specific role to a user.")]
        [SwaggerResponse(200, "Role assigned to user successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid user or role data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "User or role not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> AssignRole(
     [FromRoute] AssignRoleToUserCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [HttpPost("RemoveRoleFromUser/{roleId}/{userId}")]

        [SwaggerOperation(Summary = "Removes a role from a user", Description = "Allows an administrator to remove a specific role from a user.")]
        [SwaggerResponse(200, "Role removed from user successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid user or role data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "User or role not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> RemoveRoleFromUser(
    [FromRoute] string roleId,
    [FromRoute] string userId)
        {
            var command = new RemoveRoleFromUserCommand(roleId, userId);

            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [HttpGet("UserIsInRole/{userId}/{roleId}")]
        [SwaggerOperation(Summary = "Checks whether a user has a role", Description = "Checks if a specific user is assigned to a specific role.")]
        [SwaggerResponse(200, "User role status retrieved successfully", type: typeof(bool))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "User or role not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> UserIsInRole(
      [FromRoute] string userId,
      [FromRoute] string roleId)
        {
            var query = new UserIsInRoleQuery(userId, roleId);

            var response = await Mediator.Send(query);

            return NewResult(response);
        }




        [HttpGet("GetUserRoles/{userId}")]
        [SwaggerOperation(Summary = "Gets user roles", Description = "Retrieves all roles assigned to a specific user.")]
        [SwaggerResponse(200, "User roles retrieved successfully", type: typeof(IList<string>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "User not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetUserRoles(
    [FromRoute] string userId)
        {
            var query = new GetUserRolesQuery(userId);

            var response = await Mediator.Send(query);

            return NewResult(response);
        }



        [HttpPost("roles/{roleId}/permissions")]
        [SwaggerOperation(Summary = "Assigns permissions to a role", Description = "Allows an administrator to assign one or more permissions to a specific role.")]
        [SwaggerResponse(200, "Permissions assigned to role successfully", type: typeof(string))]
        [SwaggerResponse(400, "Invalid permission or role data")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Role or permission not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> AssignPermissionToRole(
    [FromRoute] string roleId,
    [FromBody] List<string> permissionIds)
        {
            var command = new AssignPermissionToRoleCommand(
                permissionIds,
                roleId);

            var response = await Mediator.Send(command);

            return NewResult(response);
        }


        [HttpDelete("roles/{roleId}/permissions/{permissionId}")]
        [SwaggerOperation(Summary = "Removes a permission from a role", Description = "Removes a specific permission from a role.")]
        [SwaggerResponse(200, "Permission removed from role successfully", type: typeof(string))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Role or permission not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> RemovePermissionFromRole(
        [FromRoute] string roleId,
          [FromRoute] string permissionId)
        {
            var command = new RemovePermissionFromRoleCommand(
                roleId,
                permissionId);

            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpGet("roles/{roleId}/permissions/{permissionId}")]
        [SwaggerOperation(Summary = "Checks whether a role has a permission", Description = "Checks if a specific permission is assigned to a specific role.")]
        [SwaggerResponse(200, "Role permission status retrieved successfully", type: typeof(bool))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Role or permission not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> RoleHasPermission(
    [FromRoute] string roleId,
    [FromRoute] string permissionId)
        {
            var command = new RoleHasPermissionQuery(
                roleId,
                permissionId);

            var response = await Mediator.Send(command);

            return NewResult(response);
        }

        [HttpGet("roles/{roleId}/permissions")]
        [SwaggerOperation(Summary = "Gets role permissions", Description = "Retrieves all permissions assigned to a specific role.")]
        [SwaggerResponse(200, "Role permissions retrieved successfully", type: typeof(IList<string>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Role not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetRolePermissions(
    [FromRoute] string roleId)
        {
            var query = new GetRolePermissionsQuery(roleId);

            var response = await Mediator.Send(query);

            return StatusCode((int)response.StatusCode, response);
        }


        [HttpGet("permissions/{permissionId}/roles")]
        [SwaggerOperation(Summary = "Gets roles assigned to a permission", Description = "Retrieves all roles that have a specific permission.")]
        [SwaggerResponse(200, "Permission roles retrieved successfully", type: typeof(IList<string>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Permission not found")]
        [SwaggerResponse(500, "An unexpected error occurred")]

        public async Task<IActionResult> GetPermissionRoles(
    [FromRoute] string permissionId)
        {
            var query = new GetPermissionRolesQuery(permissionId);

            var response = await Mediator.Send(query);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}
