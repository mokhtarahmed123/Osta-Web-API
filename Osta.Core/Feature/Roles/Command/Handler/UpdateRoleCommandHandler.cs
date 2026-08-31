using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Roles.Command.Model;
using Osta.Identity.Roles;
using Osta.SharedKernel.Logging;

namespace Osta.Core.Feature.Roles.Command.Handler
{
    public class UpdateRoleCommandHandler : ResponseHandler, IRequestHandler<UpdateRoleCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly IRoleService roleService;
        private readonly ILoggerService loggerService;

        public UpdateRoleCommandHandler(IMapper mapper, IRoleService roleService, ILoggerService loggerService)
        {
            this.mapper = mapper;
            this.roleService = roleService;
            this.loggerService = loggerService;
        }

        public async Task<Response<string>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await roleService.GetRoleByIdAsync(request.RoleId);

                if (role is null)
                {
                    return NotFound<string>("Role not found.");
                }

                mapper.Map(request, role);
                role.UpdatedAt = DateTime.UtcNow;
                var isUpdated = await roleService.UpdateRoleAsync(request.RoleId, role);

                if (!isUpdated)
                {
                    return BadRequest<string>("Failed to update role.");
                }

                return Success("Role updated successfully.", role.Id.ToString());
            }
            catch (Exception ex)
            {
                loggerService.LogError(ex, "Error occurred while updating role with Id: {RoleId}", request.RoleId);

                return BadRequest<string>(
                    "An error occurred while updating the role."
                    );
            }
        }
    }

}
