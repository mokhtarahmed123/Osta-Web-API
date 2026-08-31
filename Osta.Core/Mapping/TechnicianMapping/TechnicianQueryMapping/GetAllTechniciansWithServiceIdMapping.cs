using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianService;
using Osta.Data.Entities.Technician;

namespace Osta.Core.Mapping.TechnicianMapping
{
    public partial class TechnicianProfile
    {
        private void GetAllTechniciansWithServiceId()
        {
            CreateMap<Technicians, GetAllTechniciansWithServiceIdResult>().
                ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)).
                ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsVerified)).

                ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio)).
                ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating)).
                ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.TotalReviews)).
                ForMember(dest => dest.CompletedBookings, opt => opt.MapFrom(src => src.CompletedBookings)).
                ForMember(dest => dest.YearsOfExperience, opt => opt.MapFrom(src => src.YearsOfExperience)).
                ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt)).
                ForMember(dest => dest.ReasonOfReject, opt => opt.MapFrom(src => src.ReasonOfReject)).
                ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ServiceName, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore())
            ;



        }
    }
}
