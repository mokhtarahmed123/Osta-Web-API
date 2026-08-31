using Osta.Core.Feature.Roles.Command.Model;
using Osta.Data.Entities.Identity;

namespace Osta.Core.Mapping.RoleMapping
{
    public partial class RoleProfile
    {
        private void UpdateCommandMapping()
        {
            CreateMap<UpdateRoleCommand, Role>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoleName));
        }
    }
}
