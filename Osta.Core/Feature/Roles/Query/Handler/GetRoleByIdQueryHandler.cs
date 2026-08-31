using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Roles.Query.Model;
using Osta.Core.Feature.Roles.Query.Result;
using Osta.Identity.Roles;

namespace Osta.Core.Feature.Roles.Query.Handler
{
    public class GetRoleByIdQueryHandler : ResponseHandler, IRequestHandler<GetRoleByIdQuery, Response<GetRoleByIdResult>>
    {
        private readonly IRoleService roleService;
        private readonly IMapper mapper;

        public GetRoleByIdQueryHandler(IRoleService roleService, IMapper mapper)
        {
            this.roleService = roleService;
            this.mapper = mapper;
        }
        public async Task<Response<GetRoleByIdResult>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await roleService.GetRoleByIdAsync(request.Id);

            if (role is null)
            {
                return NotFound<GetRoleByIdResult>("Role not found.");
            }

            var result = mapper.Map<GetRoleByIdResult>(role);

            return Success(result);
        }
    }
}
