using Osta.Core.Feature.Service.Command.Model;

namespace Osta.Core.Mapping.ServiceMapping
{
    public partial class ServiceProfile
    {
        private void AddServiceCommandMapping()
        {
            CreateMap<AddServiceCommand, Data.Entities.Services.Service>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());
        }
    }
}
