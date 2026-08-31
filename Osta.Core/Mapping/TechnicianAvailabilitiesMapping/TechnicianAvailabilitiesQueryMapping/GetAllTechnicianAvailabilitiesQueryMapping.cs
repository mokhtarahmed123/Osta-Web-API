using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianAvailabilities;
using Osta.Data.Entities.Technician;

namespace Osta.Core.Mapping.TechnicianAvailabilitiesMapping
{
    public partial class TechnicianAvailabilitiesProfile
    {
        private void GetAll()
        {
            CreateMap<TechnicianAvailability, GetAllTechnicianAvailabilitiesResult>()
                .ForMember(dest => dest.TechnicianId, opt => opt.MapFrom(src => src.TechnicianId))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.Day, opt => opt.MapFrom(src => src.DayOfWeek.ToString()))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime));

        }
    }
}
