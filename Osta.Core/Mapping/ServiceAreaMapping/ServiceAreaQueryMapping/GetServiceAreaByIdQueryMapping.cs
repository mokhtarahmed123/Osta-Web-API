using Osta.Core.Feature.ServiceArea.Query.Result;
using Osta.Data.Entities.Technician;

namespace Osta.Core.Mapping.ServiceAreaMapping
{
    public partial class ServiceAreaProfile
    {
        private void GetById()
        {
            CreateMap<ServiceArea, GetServiceAreaByIdResult>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
               ;
        }

    }
}
