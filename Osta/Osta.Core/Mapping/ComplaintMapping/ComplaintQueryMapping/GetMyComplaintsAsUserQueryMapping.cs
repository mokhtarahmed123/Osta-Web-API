using Osta.Core.Feature.Complaint.Query.Result;
using Osta.Data.Entities.Administration;

namespace Osta.Core.Mapping.ComplaintMapping
{
    public partial class ComplaintProfile
    {
        private void GetMyComplaintsAsUser()
        {
            CreateMap<Complaint, GetMyComplaintsAsUserResult>()
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.Booking.BookingDate))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Booking.CustomerId))
                .ForMember(dest => dest.TechnicianId, opt => opt.MapFrom(src => src.Booking.TechnicianId))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));


        }
    }
}
