using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Roles.Command.Model;
using Osta.Data.Entities.Identity;
using Osta.Identity.Roles;

namespace Osta.Core.Feature.Roles.Command.Handler
{
    public class AddRoleCommandHandler : ResponseHandler, IRequestHandler<AddRoleCommand, Response<string>>
    {
        private readonly IMapper mapper;
        private readonly IRoleService roleService;


        public AddRoleCommandHandler(IMapper mapper, IRoleService roleService)
        {
            this.mapper = mapper;
            this.roleService = roleService;

        }

        public async Task<Response<string>> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = mapper.Map<Role>(request);
                role.CreatedAt = DateTime.UtcNow;
                var result = await roleService.CreateRoleAsync(role);

                if (!result)
                {
                    return BadRequest<string>(" Failed to Add Role.");
                }
                return Success<string>("Role Added Successfully.", role.Id.ToString());

            }
            catch (Exception ex)
            {
                return ServerError<string>($"An error occurred while processing your request. ,{ex.Message}");
            }
        }

    }
}
