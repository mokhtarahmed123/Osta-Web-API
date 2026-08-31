using Microsoft.AspNetCore.Identity;

namespace Osta.Identity.Authorization
{
    public interface IAuthorizationService
    {

        Task<IdentityResult> AssignRoleToUserAsync(string RoleId, string UserId);
        Task<IdentityResult> RemoveRoleFromUserAsync(string RoleId, string UserId);
        Task<bool> HasPermissionAsync(string UserId, string permission);
        Task<bool> IsInRoleAsync(
            string userId,
            string roleName);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<IList<string>> GetUserPermissionsAsync(string userId);

        Task AssignPermissionToRoleAsync(List<string> permissionIds, string roleId);
        Task RemovePermissionFromRoleAsync(
            string permissionId,
            string roleId);

        Task<bool> RoleHasPermissionAsync(
            string roleId,
            string permissionId);

        Task<IList<string>> GetRolePermissionsAsync(
            string roleId);

        Task<IList<string>> GetPermissionRolesAsync(
            string permissionId);

        Task<bool> PermissionExistAsync(string permissionId);
    }
}
