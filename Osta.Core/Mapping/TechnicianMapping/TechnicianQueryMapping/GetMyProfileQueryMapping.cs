using Osta.Core.Feature.Technician.Query.Result.ResultTechnician;
using Osta.Data.Entities.Technician;

namespace Osta.Core.Mapping.TechnicianMapping
{
    public partial class TechnicianProfile
    {
        private void MyProfile()
        {
            CreateMap<Technicians, GetMyProfileResult>()
                       .ForMember(
                           dest => dest.Bio,
                           opt => opt.MapFrom(src => src.Bio)
                       )
                       .ForMember(
                           dest => dest.Name,
                           opt => opt.MapFrom(src => src.User.FullName)
                       )
                       .ForMember(
                           dest => dest.Email,
                           opt => opt.MapFrom(src => src.User.Email)
                       )
                       .ForMember(
                           dest => dest.PhoneNumber,
                           opt => opt.MapFrom(src => src.User.PhoneNumber)
                       )
                       .ForMember(
                           dest => dest.IsVerified,
                           opt => opt.MapFrom(src => src.IsVerified)
                       )
                       .ForMember(
                           dest => dest.Rating,
                           opt => opt.MapFrom(src => src.Rating)
                       )
                       .ForMember(
                           dest => dest.YearsOfExperience,
                           opt => opt.MapFrom(src => src.YearsOfExperience)
                       )
                       .ForMember(
                           dest => dest.CompletedBookings,
                           opt => opt.MapFrom(src => src.CompletedBookings)
                       )
                       .ForMember(
                           dest => dest.ReasonOfReject,
                           opt => opt.MapFrom(src => src.ReasonOfReject)
                       )
                       .ForMember(
                           dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status.ToString())
                       )
                       .ForMember(
                           dest => dest.NationalId,
                           opt => opt.MapFrom(src => src.NationalId)
                       )
                       .ForMember(
                           dest => dest.TotalReviews,
                           opt => opt.MapFrom(src => src.TotalReviews)
                       )
                       .ForMember(
                           dest => dest.CreatedAt,
                           opt => opt.MapFrom(src => src.CreatedAt)
                       );

        }
    }
}
