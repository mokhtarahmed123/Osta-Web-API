using AutoMapper;

namespace Osta.Core.Mapping.RoleMapping
{
    public partial class RoleProfile : Profile
    {
        public RoleProfile()
        {
            AddRoleCommandMapping();
            UpdateCommandMapping();
            GetAllRoleQueryMapping();
            GetByIdQueryMapping();
        }
    }
}
