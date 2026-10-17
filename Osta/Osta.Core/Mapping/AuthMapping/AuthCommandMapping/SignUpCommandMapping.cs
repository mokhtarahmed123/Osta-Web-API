using Osta.Core.Feature.Authentication.Command.Model.AuthModel;
using Osta.Data.Entities.Identity;

namespace Osta.Core.Mapping.AuthMapping
{
    public partial class AuthProfile
    {
        private void SignUpCommandMapping()
        {
            CreateMap<SignUpCommand, User>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Governorate, opt => opt.MapFrom(src => src.Governorate))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false))
;
        }
    }
}