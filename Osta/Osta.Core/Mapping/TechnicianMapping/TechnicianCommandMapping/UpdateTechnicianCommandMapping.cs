using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Data.Entities.Technician;

namespace Osta.Core.Mapping.TechnicianMapping
{
    public partial class TechnicianProfile
    {
        private void UpdateTechnician()
        {
            CreateMap<UpdateTechnicianCommand, Technicians>()
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
             .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId))
                .ForMember(dest => dest.TechnicianServiceArea, opt => opt.Ignore())
                .ForMember(dest => dest.YearsOfExperience, opt => opt.MapFrom(src => src.YearsOfExperience))

            ;

        }
    }
}
