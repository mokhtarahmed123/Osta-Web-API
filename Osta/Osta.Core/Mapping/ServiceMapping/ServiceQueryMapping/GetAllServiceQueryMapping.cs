using Osta.Core.Feature.Service.Query.Result;

namespace Osta.Core.Mapping.ServiceMapping
{
    public partial class ServiceProfile
    {
        private void GetAllServiceQueryMapping()
        {
            CreateMap<Data.Entities.Services.Service, GetAllServiceResult>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));
        }
    }
}
