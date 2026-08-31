using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianAvailabilities;
using Osta.Data.Entities.Technician;

namespace Osta.Core.Mapping.TechnicianAvailabilitiesMapping
{
    public partial class TechnicianAvailabilitiesProfile
    {
        private void Add()
        {
            CreateMap<RequestTechnicianAvailabilityCommand, TechnicianAvailability>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime));
        }
    }
}
