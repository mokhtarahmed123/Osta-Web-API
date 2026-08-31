using Osta.Data.Entities.Identity;

namespace Osta.Identity.Roles
{
    public interface IRoleService
    {
        Task<bool> CreateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(string Id);
        Task<bool> UpdateRoleAsync(string Id, Role role);
        Task<Role?> GetRoleByIdAsync(string roleId);
        Task<bool> RoleExistsAsync(string roleId);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<IEnumerable<Role>> GetAllRolesAsync();
    }
}
