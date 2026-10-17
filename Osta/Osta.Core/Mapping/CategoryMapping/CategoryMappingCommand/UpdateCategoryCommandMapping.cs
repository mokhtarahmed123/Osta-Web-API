using Osta.Core.Feature.Category.Command.Model;
using Osta.Data.Entities.Services;

namespace Osta.Core.Mapping.CategoryMapping
{
    public partial class CategoryProfile
    {
        private void Update()
        {
            CreateMap<UpdateCategoryCommand, Category>()
             .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))

             ;
        }
    }
}
