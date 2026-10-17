using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Bases;
using Osta.Core.Feature.Authorization.Command.Model.Roles;
using Osta.Data.Entities.Identity;
using Osta.Identity.Authorization;

namespace Osta.Core.Feature.Authorization.Command.Handler
{
    public class AssignRoleToUserCommandHandler : ResponseHandler, IRequestHandler<AssignRoleToUserCommand, Response<string>>
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IAuthorizationService authorizationService;

        public AssignRoleToUserCommandHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IAuthorizationService authorizationService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.authorizationService = authorizationService;
        }
        public async Task<Response<string>> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                return NotFound<string>("User not found");
            }

            var role = await roleManager.FindByIdAsync(request.RoleId);

            if (role == null)
            {
                return NotFound<string>("Role not found");
            }
            var result = await authorizationService.AssignRoleToUserAsync(role.Id, user.Id);
            if (!result.Succeeded)
            {
                return BadRequest<string>("Failed to assign role");
            }

            return Success<string>("Role assigned successfully");
        }
    }
}
