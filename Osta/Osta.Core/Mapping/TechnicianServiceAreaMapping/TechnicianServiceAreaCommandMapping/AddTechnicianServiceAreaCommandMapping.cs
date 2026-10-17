using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianServiceArea;
using Osta.Data.Entities.Technician;

namespace Osta.Core.Mapping.TechnicianServiceAreaMapping
{
    public partial class TechnicianServiceAreaProfile
    {
        private void Add()
        {
            CreateMap<AddTechnicianServiceAreaCommand, TechnicianServiceArea>()
                .ForMember(dest => dest.ServiceAreaId, opt => opt.MapFrom(src => src.ServiceAreaId))

;
        }
    }
}
