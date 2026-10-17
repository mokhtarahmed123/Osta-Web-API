using Osta.Core.Feature.Technician.Command.Model.TechnicianModel;
using Osta.Data.Entities.Technician;
using Osta.Data.Enum;

namespace Osta.Core.Mapping.TechnicianMapping
{
    public partial class TechnicianProfile
    {
        private void Add()
        {
            CreateMap<AddTechnicianCommand, Technicians>()
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId))

                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(x => false))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(x => 0))
                .ForMember(dest => dest.CompletedBookings, opt => opt.MapFrom(x => 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(x => StatusOfTechnicianRequestEnum.Pending))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(x => DateTime.UtcNow))
                .ForMember(dest => dest.TechnicianServiceArea, opt => opt.Ignore())
            .ForMember(dest => dest.ReasonOfReject,
                      opt => opt.MapFrom(x => (string?)null))
                .ForMember(dest => dest.YearsOfExperience, opt => opt.MapFrom(src => src.YearsOfExperience))
        ;
        }
    }
}
