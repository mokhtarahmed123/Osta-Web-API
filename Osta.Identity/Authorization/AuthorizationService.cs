using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Identity;
using Osta.Domain.Entities.Identity;
using Osta.Infrastructure.DataBase;
using Osta.Notification.Interfaces;
using System.Data;

namespace Osta.Identity.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IEmailService emailService;
        private readonly OstaContext appDbContext;

        public AuthorizationService(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IEmailService emailService,
            OstaContext appDbContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.emailService = emailService;
            this.appDbContext = appDbContext;
        }


        public async Task<IdentityResult> AssignRoleToUserAsync(
            string roleId,
            string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return (IdentityResult.Failed());

            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null) return (IdentityResult.Failed());

            if (string.IsNullOrWhiteSpace(role.Name))
            {
                return IdentityResult.Failed();
            }
            var result = await userManager.AddToRoleAsync(
                user,
                role.Name);
            return result;
        }
        public async Task<IdentityResult> RemoveRoleFromUserAsync(
            string roleId,
            string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return (IdentityResult.Failed());

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null) return (IdentityResult.Failed());

            if (role.Name == null)
                return (IdentityResult.Failed());

            var result = await userManager.RemoveFromRoleAsync(
                user,
                role.Name);

            if (!result.Succeeded)
            {
                return result;
            }

            var addDefaultRoleResult =
                await userManager.AddToRoleAsync(user, "User");

            return addDefaultRoleResult;
        }

        public async Task<bool> IsInRoleAsync(
            string userId,
            string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            return await userManager.IsInRoleAsync(
                user,
                roleName);
        }
        public async Task<IList<string>> GetUserRolesAsync(
            string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return new List<string>();

            return await userManager.GetRolesAsync(user);
        }


        public async Task<bool> HasPermissionAsync(
            string userId,
            string permission)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            var roles = await userManager.GetRolesAsync(user);

            return await appDbContext.RolePermissions
                .AnyAsync(x =>
               x.PermissionId.ToString() == permission &&
                x.Role.Name != null &&
               roles.Contains(x.Role.Name))
                      ;

        }



        public async Task<IList<string>> GetUserPermissionsAsync(
            string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return new List<string>();

            var roles = await userManager.GetRolesAsync(user);


            return await appDbContext.RolePermissions
         .Where(rp =>
             rp.Role.Name != null &&
             roles.Contains(rp.Role.Name))
         .Select(rp => rp.PermissionId.ToString())
         .ToListAsync();
        }



        public async Task AssignPermissionToRoleAsync(List<string> permissionIds, string roleId)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(roleId);
                if (role == null) return;

                var permissions = await appDbContext.Permissions
            .Where(p => permissionIds.Contains(p.Id.ToString()))
            .ToListAsync();

                if (permissions.Count != permissionIds.Count)
                    throw new KeyNotFoundException("One or more permissions were not found.");

                var rolePermissions = permissions.Select(permission => new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permission.Id
                }).ToList();

                await appDbContext.RolePermissions.AddRangeAsync(rolePermissions);

                await appDbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while assigning permissions to the role.", ex);
            }
        }

        public async Task RemovePermissionFromRoleAsync(
            string permissionId,
            string roleId)
        {
            var rolePermission = await appDbContext.RolePermissions
                .FirstOrDefaultAsync(x =>
                    x.RoleId == roleId &&
                    x.PermissionId.ToString() == permissionId);

            if (rolePermission == null)
                throw new KeyNotFoundException(
                    "This permission is not assigned to this role.");

            appDbContext.RolePermissions.Remove(rolePermission);

            await appDbContext.SaveChangesAsync();
        }



        public async Task<bool> RoleHasPermissionAsync(
            string roleId,
            string permissionId)
        {
            return await appDbContext.RolePermissions
                .AnyAsync(x =>
                    x.RoleId == roleId &&
                    x.PermissionId.ToString() == permissionId);
        }
        public async Task<IList<string>> GetRolePermissionsAsync(
            string roleId)
        {
            return await appDbContext.RolePermissions
                .Where(x => x.RoleId == roleId)
                .Select(x => x.PermissionId.ToString())
                .ToListAsync();
        }
        public async Task<IList<string>> GetPermissionRolesAsync(
            string permissionId)
        {
            return await appDbContext.RolePermissions
                .Where(x => x.PermissionId.ToString() == permissionId)
                .Select(x => x.RoleId)
                .ToListAsync();
        }

        public Task<bool> PermissionExistAsync(string permissionId)
        {
            return appDbContext.Permissions.Where(x => x.Id.ToString() == permissionId).AnyAsync();
        }
    }
}