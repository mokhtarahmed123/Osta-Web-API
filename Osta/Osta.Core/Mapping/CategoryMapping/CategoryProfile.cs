using AutoMapper;

namespace Osta.Core.Mapping.CategoryMapping
{
    public partial class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            AddCategoryCommandMapping();
            GetCategoryByIdQueryMapping();
            GetAllCategoryQueryMapping();
            Update();
        }
    }
}
