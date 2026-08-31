using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Osta.API.Bases;
using Osta.Core.Feature.Authorization.Command.Model.PermissionModel;
using Osta.Core.Feature.Authorization.Command.Model.Roles;
using Osta.Core.Feature.Authorization.Query.Model;
using Osta.Core.Feature.Authorization.Query.Model.PermissionModel;

namespace Osta.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class AuthorizationController : AppBaseController
    {
        [HttpPost("AssignRole/{UserId}/{RoleId}")]
        public async Task<IActionResult> AssignRole(
     [FromRoute] AssignRoleToUserCommand command)
        {
            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [HttpPost("RemoveRoleFromUser/{roleId}/{userId}")]
        public async Task<IActionResult> RemoveRoleFromUser(
    [FromRoute] string roleId,
    [FromRoute] string userId)
        {
            var command = new RemoveRoleFromUserCommand(roleId, userId);

            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [HttpGet("UserIsInRole/{userId}/{roleId}")]
        public async Task<IActionResult> UserIsInRole(
      [FromRoute] string userId,
      [FromRoute] string roleId)
        {
            var query = new UserIsInRoleQuery(userId, roleId);

            var response = await Mediator.Send(query);

            return NewResult(response);
        }
        [HttpGet("GetUserRoles/{userId}")]
        public async Task<IActionResult> GetUserRoles(
    [FromRoute] string userId)
        {
            var query = new GetUserRolesQuery(userId);

            var response = await Mediator.Send(query);

            return NewResult(response);
        }
        [HttpPost("roles/{roleId}/permissions")]
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
        public async Task<IActionResult> GetRolePermissions(
    [FromRoute] string roleId)
        {
            var query = new GetRolePermissionsQuery(roleId);

            var response = await Mediator.Send(query);

            return StatusCode((int)response.StatusCode, response);
        }
        [HttpGet("permissions/{permissionId}/roles")]
        public async Task<IActionResult> GetPermissionRoles(
    [FromRoute] string permissionId)
        {
            var query = new GetPermissionRolesQuery(permissionId);

            var response = await Mediator.Send(query);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}
