using AutoMapper;
using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Roles.Query.Model;
using Osta.Core.Feature.Roles.Query.Result;
using Osta.Identity.Roles;

namespace Osta.Core.Feature.Roles.Query.Handler
{
    public class GetAllRolesQueryHandler : ResponseHandler, IRequestHandler<GetAllRolesQuery, Response<List<GetAllRolesResult>>>

    {
        private readonly IRoleService roleService;
        private readonly IMapper mapper;

        public GetAllRolesQueryHandler(IRoleService roleService, IMapper mapper)
        {
            this.roleService = roleService;
            this.mapper = mapper;
        }
        public async Task<Response<List<GetAllRolesResult>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await roleService.GetAllRolesAsync();

            var result = mapper.Map<List<GetAllRolesResult>>(roles);

            return Success(result);
        }
    }
}
