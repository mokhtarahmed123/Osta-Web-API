using Osta.Core.Feature.Category.Query.Result;
using Osta.Data.Entities.Services;

namespace Osta.Core.Mapping.CategoryMapping
{
    public partial class CategoryProfile
    {
        private void GetCategoryByIdQueryMapping()
        {
            CreateMap<Category, GetCategoryByIdResult>()
                 .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                 .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
