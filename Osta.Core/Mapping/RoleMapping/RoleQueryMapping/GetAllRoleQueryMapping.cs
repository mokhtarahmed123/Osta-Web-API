using Osta.Core.Feature.Roles.Query.Result;
using Osta.Data.Entities.Identity;

namespace Osta.Core.Mapping.RoleMapping
{
    public partial class RoleProfile
    {
        private void GetAllRoleQueryMapping()
        {
            CreateMap<Role, GetAllRolesResult>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}
