using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Bases;
using Osta.Core.Feature.Authorization.Query.Model;
using Osta.Core.Feature.Authorization.Query.Model.PermissionModel;
using Osta.Data.Entities.Identity;
using Osta.Identity.Authorization;

namespace Osta.Core.Feature.Authorization.Query.Handler
{
    public class AuthorizationQueryHandler : ResponseHandler,
        IRequestHandler<UserIsInRoleQuery, Response<bool>>,
        IRequestHandler<GetUserRolesQuery, Response<IList<string>>>,
        IRequestHandler<RoleHasPermissionQuery, Response<bool>>,
        IRequestHandler<GetRolePermissionsQuery, Response<IList<string>>>,
        IRequestHandler<GetPermissionRolesQuery, Response<IList<string>>>

    {
        protected readonly UserManager<User> userManager;
        protected readonly RoleManager<Role> roleManager;
        protected readonly IAuthorizationService authorizationService;

        public AuthorizationQueryHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IAuthorizationService authorizationService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.authorizationService = authorizationService;
        }
        public async Task<Response<bool>> Handle(UserIsInRoleQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                return NotFound<bool>("User not found");
            }

            var role = await roleManager.FindByIdAsync(request.RoleId);

            if (role == null)
            {
                return NotFound<bool>("Role not found");
            }
            var isInRole = await authorizationService.IsInRoleAsync(request.UserId, role.Name);
            if (isInRole)
            {
                return Success(true, $"User is in role {role.Name}");
            }
            else
            {
                return Success(false, $"User is not in role `{role.Name}`");
            }
        }
        public async Task<Response<IList<string>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await authorizationService.GetUserRolesAsync(request.UserId);

            if (roles is null)
                return NotFound<IList<string>>("User not found.");

            return Success<IList<string>>(roles);
        }

        public async Task<Response<bool>> Handle(RoleHasPermissionQuery request, CancellationToken cancellationToken)
        {
            var result = await authorizationService.RoleHasPermissionAsync(
                request.RoleId,
                request.PermissionId);

            return Success(result);
        }

        public async Task<Response<IList<string>>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
        {
            var role = await roleManager.FindByIdAsync(request.roleId);
            if (role == null)
                return NotFound<IList<string>>($" Role With Id {request.roleId} Not Found ");

            var permissions = await authorizationService.GetRolePermissionsAsync(
       request.roleId);

            return Success(permissions);
        }

        public async Task<Response<IList<string>>> Handle(GetPermissionRolesQuery request, CancellationToken cancellationToken)
        {
            var Permission = await authorizationService.PermissionExistAsync(request.PermissionId);
            if (!Permission) return NotFound<IList<string>>(" Permission Not Found ");
            var Roles = await authorizationService.GetPermissionRolesAsync(request.PermissionId);
            return Success(Roles);


        }
    }
}
