using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Roles.Command.Model;
using Osta.Identity.Roles;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Roles.Command.Handler
{
    public class DeleteRoleCommandHandler : ResponseHandler, IRequestHandler<DeleteRoleCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly IRoleService roleService;
        private readonly ILoggerService loggerService;

        public DeleteRoleCommandHandler(IMapper mapper, IRoleService roleService, ILoggerService loggerService)
        {
            this.mapper = mapper;
            this.roleService = roleService;
            this.loggerService = loggerService;
        }

        public async Task<Response<string>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await roleService.GetRoleByIdAsync(request.RoleId);

                if (role is null)
                {
                    return NotFound<string>("Role not found.");
                }

                var result = await roleService.DeleteRoleAsync(request.RoleId);
                if (!result)
                {
                    return BadRequest<string>("Failed to delete role.");
                }

                return Success("Role deleted successfully.", role.Id.ToString());
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "Error occurred while deleting role with Id: {RoleId}", request.RoleId);

                return BadRequest<string>(
                    "An error occurred while deleting the role."
                   );
            }
        }
    }
}
