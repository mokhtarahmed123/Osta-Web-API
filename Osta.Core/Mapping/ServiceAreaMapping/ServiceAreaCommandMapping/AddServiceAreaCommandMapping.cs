using Osta.Core.Feature.ServiceArea.Command.Model;
using Osta.Data.Entities.Technician;

namespace Osta.Core.Mapping.ServiceAreaMapping
{
    public partial class ServiceAreaProfile
    {
        private void Add()
        {
            CreateMap<AddServiceAreaCommand, ServiceArea>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
               ;
        }
    }
}
