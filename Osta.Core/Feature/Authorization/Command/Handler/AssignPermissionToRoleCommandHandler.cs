using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Bases;
using Osta.Core.Feature.Authorization.Command.Model.PermissionModel;
using Osta.Data.Entities.Identity;
using Osta.Identity.Authorization;

namespace Osta.Core.Feature.Authorization.Command.Handler
{
    public class AssignPermissionToRoleCommandHandler : ResponseHandler, IRequestHandler<AssignPermissionToRoleCommand, Response<string>>
    {

        private readonly RoleManager<Role> roleManager;
        private readonly IAuthorizationService authorizationService;

        public AssignPermissionToRoleCommandHandler(RoleManager<Role> roleManager, IAuthorizationService authorizationService)
        {
            this.roleManager = roleManager;
            this.authorizationService = authorizationService;
        }
        public async Task<Response<string>> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await roleManager.FindByIdAsync(request.RoleId);
            if (role == null)
                return NotFound<string>("Role not found");


            await authorizationService.AssignPermissionToRoleAsync(
                  request.PermissionIds,
                  request.RoleId);

            return Success<string>("Permissions assigned to role successfully.");
        }

    }
}
