using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Bases;
using Osta.Core.Feature.Authorization.Command.Model.Roles;
using Osta.Data.Entities.Identity;
using Osta.Identity.Authorization;

namespace Osta.Core.Feature.Authorization.Command.Handler
{
    public class RemoveRoleFromUserCommandHandler : ResponseHandler, IRequestHandler<RemoveRoleFromUserCommand, Response<string>>
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IAuthorizationService authorizationService;

        public RemoveRoleFromUserCommandHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IAuthorizationService authorizationService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.authorizationService = authorizationService;
        }
        public async Task<Response<string>> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.userId);

            if (user == null)
            {
                return NotFound<string>("User not found");
            }

            var role = await roleManager.FindByIdAsync(request.roleId);

            if (role == null)
            {
                return NotFound<string>("Role not found");
            }
            var result = await authorizationService.RemoveRoleFromUserAsync(role.Id, user.Id);
            if (!result.Succeeded)
            {
                return BadRequest<string>("Failed to remove role");
            }
            return Success<string>("Role removed successfully");

        }
    }
}
