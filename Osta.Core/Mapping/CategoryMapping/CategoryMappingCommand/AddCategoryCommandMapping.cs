using Osta.Core.Feature.Category.Command.Model;
using Osta.Data.Entities.Services;

namespace Osta.Core.Mapping.CategoryMapping
{
    public partial class CategoryProfile
    {
        private void AddCategoryCommandMapping()
        {
            CreateMap<AddCategoryCommand, Category>()
                 .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
        }
    }
}
