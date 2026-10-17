using Osta.Core.Feature.Roles.Query.Result;
using Osta.Data.Entities.Identity;

namespace Osta.Core.Mapping.RoleMapping
{
    public partial class RoleProfile
    {
        private void GetByIdQueryMapping()
        {
            CreateMap<Role, GetRoleByIdResult>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}
