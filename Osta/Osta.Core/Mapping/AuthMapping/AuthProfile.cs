using AutoMapper;

namespace Osta.Core.Mapping.AuthMapping
{
    public partial class AuthProfile : Profile
    {
        public AuthProfile()
        {
            SignUpCommandMapping();
        }
    }
}
