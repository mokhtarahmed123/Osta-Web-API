using Osta.Core.Feature.Roles.Command.Model;
using Osta.Data.Entities.Identity;

namespace Osta.Core.Mapping.RoleMapping
{
    public partial class RoleProfile
    {
        private void AddRoleCommandMapping()
        {
            CreateMap<AddRoleCommand, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}