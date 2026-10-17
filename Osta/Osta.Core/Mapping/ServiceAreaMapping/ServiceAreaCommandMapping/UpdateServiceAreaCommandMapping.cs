using Osta.Core.Feature.ServiceArea.Command.Model;
using Osta.Data.Entities.Technician;

namespace Osta.Core.Mapping.ServiceAreaMapping
{
    public partial class ServiceAreaProfile
    {
        private void Update()
        {
            CreateMap<UpdateServiceAreaCommand, ServiceArea>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               ;
        }

    }
}
