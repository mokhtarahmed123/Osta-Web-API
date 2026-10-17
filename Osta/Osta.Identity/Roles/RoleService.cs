using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Osta.Core.HandlerMiddleware;
using Osta.Data.Entities.Identity;

namespace Osta.Identity.Roles
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;


        public RoleService(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;

        }
        public async Task<bool> CreateRoleAsync(Role role)
        {
            var Result = await roleManager.CreateAsync(role);
            if (Result.Succeeded) return true;
            return false;

        }

        public async Task<bool> DeleteRoleAsync(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);
            if (role is null)
            {
                return false;
            }

            var result = await roleManager.DeleteAsync(role);

            if (result.Succeeded)
                return true;
            return false;

        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return roles;
        }

        public async Task<Role?> GetRoleByIdAsync(string roleId)
        {
            return await roleManager.FindByIdAsync(roleId);

        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            var user = userManager.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) throw new NotFoundException("User not found");
            var roles = await userManager.GetRolesAsync(user);
            return roles;

        }

        public async Task<bool> RoleExistsAsync(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null) return false;
            return (true);

        }

        public async Task<bool> UpdateRoleAsync(string Id, Role role)
        {
            var existingRole = await roleManager.FindByIdAsync(Id);
            if (existingRole == null)
            {
                throw new NotFoundException("Role not found");
            }
            existingRole.Name = role.Name;
            var result = await roleManager.UpdateAsync(existingRole);
            if (result.Succeeded)
                return true;

            return false;
        }
    }
}
