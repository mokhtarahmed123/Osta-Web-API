using Osta.Core.Feature.Service.Query.Result;

namespace Osta.Core.Mapping.ServiceMapping
{
    public partial class ServiceProfile
    {
        private void GetById()
        {
            CreateMap<Osta.Data.Entities.Services.Service, GetServiceByIdResult>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));
        }
    }
}
